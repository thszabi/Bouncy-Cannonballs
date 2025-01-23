using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BricksController))]
[RequireComponent(typeof(ScoreController))]
[RequireComponent(typeof(GameUiController))]
public class GameSceneController : MonoBehaviour
{
    [SerializeField] private Transform _levelTransform;
    [SerializeField] private BallsController _ballsController;
    [SerializeField] private LineRenderer _line1;
    [SerializeField] private LineRenderer _line2;
    [SerializeField] private float _speedUpThreshold = 5.0f;
    [SerializeField] private float _speedUpMultiplier = 2.0f;

    private readonly float BallRadius = 0.101f;
    private readonly float LevelWidth = 18.0f;
    private readonly float LevelHeight = 10.0f;

    private BricksController _bricksController;
    private ScoreController _scoreController;
    private GameUiController _uiController;

    private Vector2? _buttonPressedPosition = null;
    private bool _isGameOver;
    private bool _isInputLocked;
    private float _roundTime;

    private void Awake()
    {
        float screenAspect = Camera.main.aspect;
        float targetAspect = LevelWidth / LevelHeight;

        if (screenAspect >= targetAspect)
        {
            Camera.main.orthographicSize = LevelHeight / 2.0f;
        }
        else
        {
            Camera.main.orthographicSize = (LevelWidth / screenAspect) / 2.0f;
        }
    }

    private void Start()
    {
        Physics2D.queriesStartInColliders = false;
        GlobalController.Instance.IsGameOver = false;
        GlobalController.Instance.LevelPlayerScore = 0;

        _isGameOver = false;
        _isInputLocked = false;

        _bricksController = GetComponent<BricksController>();
        _scoreController = GetComponent<ScoreController>();
        _uiController = GetComponent<GameUiController>();

        _ballsController.BallsStoppedEvent += OnRoundEnded;
        _bricksController.BrickDestroyedEvent += OnBrickDestroyed;
        _scoreController.ScoreChangedEvent += OnScoreChanged;
        _uiController.RetrieveBallsButtonClickedEvent += OnRetrieveBallsButtonClicked;

        _uiController.SetRetrieveBallsButtonInteractable(false);

        LevelResource levelResource = LevelResource.Load(GlobalController.Instance.CurrentLevel);
        if (levelResource != null && levelResource.LevelData.InitialBallsCount > 0)
        {
            Instantiate(levelResource.LevelData.gameObject, _levelTransform);

            GameObject floorObject = GameObject.FindGameObjectWithTag("Floor");
            if (floorObject != null)
            {
                float initialBallPositionX = floorObject.transform.position.x;
                float initialBallPositionY = floorObject.transform.position.y + (floorObject.transform.lossyScale.y / 2.0f) + BallRadius;
                Vector2 initialBallPosition = new Vector2(initialBallPositionX, initialBallPositionY);

                _ballsController.Initialize(levelResource.LevelData.InitialBallsCount, initialBallPosition);
                _uiController.Initialize(_scoreController.GetScore(), levelResource.LevelData.InitialBallsCount);
            }
        }

    }

    private void Update()
    {
        UpdateInput();
        UpdateLineRenderers();
        UpdateRoundTime();
    }

    private void UpdateInput()
    {
        if (!_isGameOver && !_isInputLocked)
        {
            if (Input.GetButton("Mouse 0"))
            {
                Vector3 buttonPressedPosition3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _buttonPressedPosition = new Vector2(buttonPressedPosition3D.x, buttonPressedPosition3D.y);
            }
            else if (Input.GetButtonUp("Mouse 0") && _buttonPressedPosition.HasValue)
            {
                Vector2 ballsPosition = _ballsController.GetBallsGatheringPosition();
                Vector2 movingDirection = _buttonPressedPosition.Value - ballsPosition;
                _ballsController.ShootBalls(movingDirection);
                _roundTime = 0.0f;
                _isInputLocked = true;
                _uiController.SetRetrieveBallsButtonInteractable(true);

                _buttonPressedPosition = null;
            }
        }
    }

    private void UpdateLineRenderers()
    {
        if (!_buttonPressedPosition.HasValue)
        {
            _line1.enabled = false;
            _line2.enabled = false;
            return;
        }

        // First ray:
        Vector2 ballsPosition = _ballsController.GetBallsGatheringPosition();
        Vector2 shootDirection = _buttonPressedPosition.Value - ballsPosition;
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Default"));
        RaycastHit2D[] raycastHits = new RaycastHit2D[5];
        int raycastHitsLength = Physics2D.CircleCast(ballsPosition, BallRadius, shootDirection, contactFilter, raycastHits, 20.0f);

        if (raycastHitsLength == 0)
        {
            _line1.enabled = false;
            _line2.enabled = false;
            return;
        }

        _line1.enabled = true;
        _line1.SetPosition(0, new Vector3(ballsPosition.x, ballsPosition.y, 0.0f));
        _line1.SetPosition(1, new Vector3(raycastHits[0].centroid.x, raycastHits[0].centroid.y, 0.0f));

        // Second ray:
        // Find other objects hit by raycast with the same distance as the first hit object
        // (this usually happens when the player aims between two bricks that are next to each other).
        Vector2 combinedNormalVector = raycastHits[0].normal;
        float distance = raycastHits[0].distance;
        for (int i = 1; i < raycastHitsLength; i++)
        {
            float diff = distance - raycastHits[i].distance;
            if (Mathf.Abs(diff) < 0.01f)
            {
                combinedNormalVector += raycastHits[i].normal;
            }
            else
            {
                break;
            }
        }

        // Deflect the first ray, and do another cast towards the deflected direction:
        Vector2 deflectDirection = Utils.MirrorVector(shootDirection, combinedNormalVector.normalized);
        Vector2 deflectRayStartPosition = new Vector2(raycastHits[0].centroid.x, raycastHits[0].centroid.y);
        RaycastHit2D[] deflectRaycastHits = new RaycastHit2D[5];
        int deflectRaycastHitsLength = Physics2D.CircleCast(deflectRayStartPosition, BallRadius, deflectDirection, contactFilter, deflectRaycastHits, 20.0f);

        if (deflectRaycastHitsLength == 0)
        {
            _line2.enabled = false;
            return;
        }

        _line2.enabled = true;
        _line2.SetPosition(0, new Vector3(raycastHits[0].centroid.x, raycastHits[0].centroid.y, 0.0f));
        _line2.SetPosition(1, new Vector3(deflectRaycastHits[0].centroid.x, deflectRaycastHits[0].centroid.y, 0.0f));
    }

    private void UpdateRoundTime()
    {
        if (!_isGameOver && _isInputLocked)
        {
            _roundTime += Time.deltaTime;
            if (_roundTime >= _speedUpThreshold)
            {
                Time.timeScale = _speedUpMultiplier;
            }
        }
    }

    private void OnBrickDestroyed()
    {
        _scoreController.OnBrickDestroyed();
    }

    private void OnScoreChanged(int score)
    {
        _uiController.OnScoreChanged(score);
    }

    private void OnRoundEnded()
    {
        _bricksController.MoveBricksDown();
        _isInputLocked = false;
        _uiController.SetRetrieveBallsButtonInteractable(false);
        Time.timeScale = 1.0f;

        // Player won:
        if (_bricksController.IsEveryBrickDestroyed)
        {
            _isGameOver = true;
            GlobalController.Instance.LevelPlayerScore = _scoreController.GetScore();
            StartCoroutine(LoadMultiplierSceneCoroutine());
        }
        // Player lost:
        else if (_bricksController.IsBricksReachedBottom)
        {
            _isGameOver = true;
            GlobalController.Instance.IsGameOver = true;
            _ballsController.DestroyAllBalls();
            StartCoroutine(LoadMultiplierSceneCoroutine());
        }
    }

    private void OnRetrieveBallsButtonClicked()
    {
        _ballsController.RetrieveBalls();
    }

    private IEnumerator LoadMultiplierSceneCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("MultiplierScene");
    }

    private void OnDestroy()
    {
        _ballsController.BallsStoppedEvent -= OnRoundEnded;
        _bricksController.BrickDestroyedEvent -= OnBrickDestroyed;
        _scoreController.ScoreChangedEvent -= OnScoreChanged;
        _uiController.RetrieveBallsButtonClickedEvent -= OnRetrieveBallsButtonClicked;
    }
}