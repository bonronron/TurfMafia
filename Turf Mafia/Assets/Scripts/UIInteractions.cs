using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.MapLayers.Components;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIInteractions : MonoBehaviour
{
    [Header("UI Interactions")]
    public VisualElement root;
    VisualElement buildMenu;
    Label lblMoney;
    Label lblSpawnersCount;
    Button btnBuild;
    Button btnCloseBuild;
    Button btnTower1;
    Button btnTower2;
    Button btnTower3;
    Button btnShoot;
    Button btnLeftCam;
    Button btnRightCam;

    internal ProgressBar shootCooldownBar;
    [SerializeField] Color buttonSelected;
    [SerializeField] Color buttonDeselected;

    [Header("Map Interactions")]
    [SerializeField] Camera _camera;
    [SerializeField] LayerGameObjectPlacement homeTurfPlacement;
    [SerializeField] LightshipMapView lightshipMapView;
    [SerializeField] List<CinemachineVirtualCamera> virtualCameras;
    [SerializeField] GameManager gameManager;

    Vector3 touchPosition = Vector3.zero;
    public TouchType touchType = TouchType.HomeTurf;

    public TowerType towerSelected;
    [SerializeField] List<GameObject> towerDecoysList;
    [SerializeField] List<LayerGameObjectPlacement> towerPlacementList;

    [SerializeField] GameObject homeTurfDecoy;
    GameObject placedDecoy;

    [SerializeField] public GameEvent onHomeTurfPlaced;
    [SerializeField] public GameEvent onTowerPlaced;

    [SerializeField] private float shootCooldown;
    [SerializeField] internal int maxShootCooldown;

    [SerializeField] UIDocument UIDoc;
    [SerializeField] VisualTreeAsset mainUI;
    [SerializeField] VisualTreeAsset WinLoseUI;


    void Start()
    {
        UIDoc = GetComponent<UIDocument>();
        root = UIDoc.rootVisualElement;
        buildMenu = root.Q("Towers");
        buildMenu.visible = false;
        btnBuild = root.Q("Build").Q<Button>("btnBuild");
        btnCloseBuild = buildMenu.Q<Button>("btnCloseBuild");
        btnShoot = root.Q("Build").Q<Button>("btnShoot");
        shootCooldownBar = root.Q("Build").Q<ProgressBar>("ShootCooldown");
        lblMoney = root.Q<Label>("Money");
        lblSpawnersCount = root.Q<Label>("SpawnersCount");

        btnTower1 = buildMenu.Q<VisualElement>("Tower1").Q<Button>();
        btnTower2 = buildMenu.Q<VisualElement>("Tower2").Q<Button>();
        btnTower3 = buildMenu.Q<VisualElement>("Tower3").Q<Button>();

        btnBuild.pickingMode = PickingMode.Ignore;
        btnBuild.style.display = DisplayStyle.None;
        btnShoot.pickingMode = PickingMode.Ignore;
        btnShoot.style.display = DisplayStyle.None;
        shootCooldownBar.visible = false;
        shootCooldownBar.lowValue = -1;
        shootCooldownBar.highValue = maxShootCooldown - 0.5f;
        shootCooldownBar.value = 0;

        btnLeftCam = root.Q("Camera").Q<Button>("LeftCamera");
        btnRightCam = root.Q("Camera").Q<Button>("RightCamera");

        btnBuild.clicked += BtnBuild_clicked;
        btnShoot.clicked += BtnShoot_clicked;
        btnCloseBuild.clicked += BtnCloseBuild_clicked;
        btnTower1.clicked += BtnTower1_clicked;
        btnTower2.clicked += BtnTower2_clicked;
        btnTower3.clicked += BtnTower3_clicked;
        btnLeftCam.clicked += BtnLeftCam_clicked;
        btnRightCam.clicked += BtnRightCam_clicked;

        shootCooldown = maxShootCooldown;
    }



    public void Initialise()
    {
        btnBuild.style.display = DisplayStyle.Flex;
        btnBuild.pickingMode = PickingMode.Position;
        btnShoot.style.display = DisplayStyle.Flex;
        btnShoot.pickingMode = PickingMode.Position;
    }

    private void Update()
    {
        if (shootCooldown < maxShootCooldown)
        {
            if (shootCooldown == 0) shootCooldownBar.value = 0;
            shootCooldownBar.value = Mathf.Lerp(shootCooldownBar.value, shootCooldown, Time.deltaTime * 10f);
        }

    }

    void FixedUpdate()
    {
        bool touchDetected = false;
        if (Input.touchCount == 1)
        {
            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                touchPosition = Input.touches[0].position;
                touchDetected = true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            touchPosition = Input.mousePosition;
            touchDetected = true;
        }

        switch (touchType)
        {
            case TouchType.HomeTurf:
                if (touchDetected)
                {
                    if (tryPlaceHomeTurf(touchPosition))
                    {
                        onHomeTurfPlaced.Notify();
                        touchType = TouchType.Idle;
                    }
                }
                break;
            case TouchType.PlaceTower:
                if (touchDetected)
                {
                    if (tryPlaceTower(touchPosition))
                    {
                        onTowerPlaced.Notify();
                        gameManager.boughtTower(towerSelected);
                        btnTower1.style.backgroundColor = buttonDeselected;
                        btnTower2.style.backgroundColor = buttonDeselected;
                        btnTower3.style.backgroundColor = buttonDeselected;
                        touchType = TouchType.Idle;
                    }
                }
                break;
            case TouchType.Shoot:
                if (touchDetected)
                {
                    tryShootSpawner(touchPosition);
                }
                break;
            case TouchType.Idle:
                if (placedDecoy) Destroy(placedDecoy);
                break;
            default:
                Debug.LogError("Unknown TouchType", this);
                break;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
        {
            int curCam = virtualCameras.FindIndex((component) => component.enabled);
            int nextCam = Input.GetKeyDown(KeyCode.Q) ? (curCam - 1 + virtualCameras.Count) % virtualCameras.Count : (curCam + 1) % virtualCameras.Count;
            virtualCameras[nextCam].enabled = true;
            virtualCameras[curCam].enabled = false;
        }
#endif
        lblMoney.text = "$ " + gameManager.money;
        lblSpawnersCount.text = "\x2620 " + gameManager.SpawnersDestroyed + " / " + gameManager.gameData.towersToDestroyForWin;
    }

    #region Buttons Clicked
    private void BtnShoot_clicked()
    {
        if (setToShoot()) btnShoot.style.backgroundColor = buttonSelected;
        else btnShoot.style.backgroundColor = buttonDeselected;
    }
    private void BtnTower1_clicked()
    {
        if (touchType == TouchType.PlaceTower) touchType = TouchType.Idle;
        else touchType = TouchType.PlaceTower;
        btnTower1.style.backgroundColor = buttonSelected;
        towerSelected = TowerType.TommyGunTurret;
    }
    private void BtnTower2_clicked()
    {
        if (touchType == TouchType.PlaceTower) touchType = TouchType.Idle;
        else touchType = TouchType.PlaceTower;

        btnTower2.style.backgroundColor = buttonSelected;
        towerSelected = TowerType.SpeakeasyBar;
    }
    private void BtnTower3_clicked()
    {
        if (touchType == TouchType.PlaceTower) touchType = TouchType.Idle;
        else touchType = TouchType.PlaceTower;

        btnTower3.style.backgroundColor = buttonSelected;
        towerSelected = TowerType.BribeMaster;
    }
    private void BtnCloseBuild_clicked()
    {
        touchType = TouchType.Idle;
        btnBuild.visible = true;
        btnShoot.visible = true;
        buildMenu.visible = false;
    }
    private void BtnBuild_clicked()
    {
        btnBuild.visible = false;
        btnShoot.visible = false;
        buildMenu.visible = true;
    }
    private void BtnLeftCam_clicked()
    {
        int curCam = virtualCameras.FindIndex((component) => component.enabled);
        int nextCam = (curCam - 1 + virtualCameras.Count) % virtualCameras.Count;
        virtualCameras[nextCam].enabled = true;
        virtualCameras[curCam].enabled = false;
    }
    private void BtnRightCam_clicked()
    {
        int curCam = virtualCameras.FindIndex((component) => component.enabled);
        int nextCam = (curCam + 1) % virtualCameras.Count;
        virtualCameras[nextCam].enabled = true;
        virtualCameras[curCam].enabled = false;
    }

    #endregion

    #region Placing Towers Functions
    private void tryShootSpawner(Vector3 touchPosition)
    {
        if (shootCooldown < maxShootCooldown) return;
        var objectOnMap = checkTouchPositionForObjects(touchPosition);
        if (objectOnMap != null && objectOnMap.GetType() == typeof(GameObject))
        {
            StartCoroutine(resetCooldown());
            GameObject gameobjectOnMap = (GameObject)objectOnMap;
            if (!gameobjectOnMap.CompareTag("EnemySpawner")) return;
            gameobjectOnMap.GetComponent<EnemySpawner>().takeDamage(Constants.GetRandomDamage());
        }
    }
    bool tryPlaceTower(Vector3 touchPosition)
    {
        if (!gameManager.canBuy(towerSelected)) { StartCoroutine(lowMoney()); return false; }
        var objectOnMap = checkTouchPositionForObjects(touchPosition);
        if (objectOnMap != null && objectOnMap.GetType() == typeof(GameObject))
        {
            GameObject gameobjectOnMap = (GameObject)objectOnMap;
            if (gameobjectOnMap == placedDecoy)
            {
                var placement = towerPlacementList[(int)towerSelected];
                placement.PlaceInstance(lightshipMapView.SceneToLatLng(placedDecoy.transform.position), placedDecoy.transform.rotation);
                Destroy(placedDecoy);
                return true;
            }
        }
        else
        {
            if (placedDecoy) Destroy(placedDecoy);

            var clickRay = _camera.ScreenPointToRay(touchPosition);
            var pointOnMap = clickRay.origin + clickRay.direction * (-clickRay.origin.y / clickRay.direction.y);
            placedDecoy = Instantiate(towerDecoysList[(int)towerSelected], pointOnMap, Quaternion.identity);
        }
        return false;
    }
    bool tryPlaceHomeTurf(Vector3 touchPosition)
    {
        var objectOnMap = checkTouchPositionForObjects(touchPosition);
        if (objectOnMap != null && objectOnMap.GetType() == typeof(GameObject))
        {
            GameObject gameobjectOnMap = (GameObject)objectOnMap;
            if (gameobjectOnMap == placedDecoy)
            {
                homeTurfPlacement.PlaceInstance(lightshipMapView.SceneToLatLng(placedDecoy.transform.position), placedDecoy.transform.rotation);
                Destroy(placedDecoy);
                return true;
            }
        }
        else
        {
            Destroy(placedDecoy);

            var clickRay = _camera.ScreenPointToRay(touchPosition);
            var pointOnMap = clickRay.origin + clickRay.direction * (-clickRay.origin.y / clickRay.direction.y);

            placedDecoy = Instantiate(homeTurfDecoy, pointOnMap, Quaternion.identity);
        }
        return false;
    }
    object checkTouchPositionForObjects(Vector3 touchPosition)
    {
        var touchRay = _camera.ScreenPointToRay(touchPosition);

        if (!Physics.Raycast(touchRay, out var hitInfo))
        {
            Debug.DrawRay(_camera.transform.position, touchPosition - _camera.transform.position, Color.cyan);
            //Debug.Log("Ray Failed");
            return null;
        }
        else
        {
            //Debug.Log("HIT!");
            return hitInfo.collider.gameObject;
        }
    }

    #endregion

    private IEnumerator resetCooldown()
    {
        shootCooldown = 0;
        shootCooldownBar.visible = true;
        while (true)
        {

            yield return new WaitForSeconds(1 / 60);
            shootCooldown += 0.1f;

            if (shootCooldown >= maxShootCooldown) break;
        }
        shootCooldownBar.visible = false;
    }
    internal bool setToShoot()
    {
        if (touchType == TouchType.Shoot)
        {
            touchType = TouchType.Idle;
            return false;
        }
        else if (touchType == TouchType.Idle)
        {
            touchType = TouchType.Shoot;
            return true;
        }
        return false;
    }
    public void stopPlacingTower()
    {
        if (touchType == TouchType.PlaceTower) touchType = TouchType.Idle;
        if (placedDecoy) Destroy(placedDecoy);
    }

    IEnumerator lowMoney(){
        lblMoney.style.color = new StyleColor(Color.red);
        yield return new WaitForSeconds(2f);
        lblMoney.style.color = new StyleColor(Color.white);
    }

    #region UI Change for win/lose
    public void winUI()
    {
        UIDoc.visualTreeAsset = WinLoseUI;
        UIDoc.rootVisualElement.Q("Lose").style.display = DisplayStyle.None;
        UIDoc.rootVisualElement.Q("Win").style.display = DisplayStyle.Flex;
        UIDoc.rootVisualElement.Q<Button>("btnMainMenu").clicked += () => { SceneManager.LoadScene("MainMenu"); };
    }
    public void loseUI()
    {
        UIDoc.visualTreeAsset = WinLoseUI;
        UIDoc.rootVisualElement.Q("Lose").style.display = DisplayStyle.Flex;
        UIDoc.rootVisualElement.Q("Win").style.display = DisplayStyle.None;
        UIDoc.rootVisualElement.Q<Button>("btnMainMenu").clicked += () => { SceneManager.LoadScene("MainMenu"); };
        UIDoc.rootVisualElement.Q<Button>("btnTryAgain").clicked += () => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); };
    }
    #endregion
}
public enum TouchType
{
    HomeTurf,
    Idle,
    PlaceTower,
    Shoot
}