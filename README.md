<div align="center">
<br>
<img src="Assets/art/OTR.png" width="600">
<br> <br>
</div>

# Introducció
On the road és un videojoc de cotxes basat en curses contrarellotge en què el jugador competeix amb si mateix per aconseguir el millor temps. En aquesta demo hi ha tres cotxes disponibles i 2 circuits complets.
<div align="center">
<img src="ReadmeImages/Gameplay.gif" width="600">
<br> <br>
</div>

# Com es juga

## Al menú principal

<div align="center">
  <br>
  <kbd>🖱️</kbd> - Interacció amb el menú <br><br>
  <kbd>🖱️LMC</kbd> - Executar l'acció del botó <br><br>
  <kbd>A</kbd>  <kbd>D</kbd> - Durant la selecció de cotxe, canviar de cotxe <br><br>
  <kbd>Intro</kbd> - Escollir cotxe <br><br>
  <kbd>Esc</kbd> - Tornar al menú anterior

</div>

## En la cursa

<div align="center">
<br>
  <kbd>A</kbd> <kbd>D</kbd> - Girar el volant <br><br>
  <kbd>W</kbd> - Accelerar <br><br>
  <kbd>S</kbd> - Frenar / Marxa enrere <br> <br>
  <kbd>Spacebar</kbd> - Fre de mà / Derrapar (si el cotxe ho permet) <br><br>
  <kbd>Esc</kbd> - Pausar el joc
</div>

# Característiques dels circuits

En els diferents circuits podem trobar 3 tipus de terrenys:

- **Carretera:** El cotxe es desplaça amb normalitat
- **Pista:** El cotxe es desplaça lleugerament més lent i quan gira derrapa
- **Fora de pista:** El cotxe es mou considerablement més lent

A cada circuit es poden prendre dreceres fora pistes, de forma que el jugador pot valorar si mereix la pèrdua de velocitat per retallar part del circuit. Com és un prototip en què sobretot es busca experimentar els diferents comportaments del vehicle s'ha optat per mantenir el circuit amb pocs obstacles. D'aquesta manera es pot testejar amb facilitat les diferents mecàniques.

## City road

<div align="center"> 
<img src="Assets/Art/CityRoadThumbnail.png" width="600">
<br><br>
</div>

En aquest primer circuit trobem un terreny totalment planer amb un entorn urbà i una pista de terra. Hi ha dues grans dreceres en que es pot evitar gran part del circuit. S'han mantingut perquè si no es coneix bé el circuit no és del tot evident i permet al jugador que vulgui explorar tenir la recompensa de retallar temps. Per altra banda al _thumbnail_ de la selecció de circuit hi ha una pista d'on es pot trobar la primera gran drecera.

## Mountain call

<div align="center">
<img src="Assets/Art/MountainCallThumbnail.png" width="600">
<br><br>
</div>

Aquest cop, com el seu nom indica, trobem un terreny muntanyós amb desnivells i, de nou, dues grans dreceres. Una d'elles indicada amb una textura diferent tot i que el terreny d'ambdues segueixen sent **fora de pista** per penalitzar la velocitat màxima durant el pas per elles.
S'ha optat per habilitar una zona de vent a la part més alta del circuit on es pot apreciar que els arbres es mouen. També hi ha altres elements com roques i aigua, tot i que el pas fins a ells està restringit fent ús d'elements naturals que impedeixen el pas del cotxe a través.

# Funcionalitats i solucions tècniques

## Al menú inicial

<div align="center">
<img src="ReadmeImages/MainMenu.png" width="600">
<br><br>
</div>

Al menú principal es permeten diverses accions. A part de les funcionalitats bàsiques com jugar o sortir del joc, es permet al jugador reiniciar les dades guardades del joc. També cal destacar el sistema de tria de circuit on es mostra la miniatura del circuit a triar i el millor temps aconseguit. Pel que fa a la tria de vehicle es fa ús de _Cinemachine_ i una representació gràfica de les característiques de cada vehicle que s'actualitza automàticament adaptant-se al comportament real dotat al vehicle.

Per poder gestionar el flux de pantalles del menú inicial s'ha optat per una màquina d'estats finits (_FSM_), d'aquesta manera es pot gestionar el flux de moviment de la càmera entre cotxes només en la tria de vehicle, així com la gestió del panell mostrat segons l'estat actual.
_Fragment del codi:_

```csharp
private enum MenuState
{
  MainMenu,
  CircuitSelection,
  CarSelection
}

void ExecuteMenuState()
{
  switch (currentMenuState)
  {
    case MenuState.MainMenu:
      ShowMainMenu();
      break;
    case MenuState.CircuitSelection:
      ShowCircuitSelection();
      break;
    case MenuState.CarSelection:
      ShowCarSelection();
        ShowCar(currentCarIndex);
        break;
  }
}

public void NextMenuState()
{
  int maxIndex = (int)MenuState.CarSelection;
  currentMenuState = (MenuState)Mathf.Min((int)currentMenuState + 1, maxIndex);
  ExecuteMenuState();
}

public void PreviousMenuState()
{
  cars[currentCarIndex].carCamera.Priority = 10;
  currentMenuState = (MenuState)Mathf.Max((int)currentMenuState - 1, 0);
  ExecuteMenuState();
}
```

### Gestió de les dades persistents
Per tal que el jugador pugui tenir continuïtat entre partides s'ha decidit fer ús del `JsonUtility` de ***Unity*** per tal d'exportar els millors temps en un arxiu `JSON`. D'aquesta manera s'emmagatzema la informació clau en un arxiu extern que es llegeix quan el jugador torna a obrir el joc. Per poder emmagatzemar i cercar l'arxiu de forma efectiva es fa servir el path natiu de ***Unity*** fent ús de la propietat `Application.persistentDataPath`.
La informació resum i les dades de reproducció dels millors temps s'emmagatzemen en diferents arxius com es detalla a continuació.

Per tenir accés a aquesta funcionalitat de forma global i pràctica sense necessitat de crear dependències, el manager de memòria es defineix com a classe pública estàtica (`public static class DataManager`). D'aquesta manera no es depén d'un `Monobehaviour` i es pot accedir al codi sense necessitat de tenir el component associat a un `GameObject`.

*Fragment de enregistrament del resum de millors temps:*
```csharp
public static void SaveData(BestCircuitTimeSummarySO summary)
{
    BestLapDataListWrapper wrapper = new BestLapDataListWrapper();
    wrapper.items = new List<BestLapData>(summary.bestLapDataList);
    string jsonData = JsonUtility.ToJson(wrapper, true);
    string filePath = Application.persistentDataPath + "/best_circuit_time_summary.json";

    File.WriteAllText(filePath, jsonData);

    Debug.Log($"Game saved to {filePath}");
}

    public static void LoadData(BestCircuitTimeSummarySO summary)
{
    string filePath = Application.persistentDataPath + "/best_circuit_time_summary.json";

    if (File.Exists(filePath))
    {
        string jsonData = File.ReadAllText(filePath);
        BestLapDataListWrapper wrapper = JsonUtility.FromJson<BestLapDataListWrapper>(jsonData);
        summary.CopyFrom(wrapper.items);
    }
    else
    {
        Debug.LogWarning($"Save file not found at {filePath}");
    }
}
```

Com a detall important, ressaltar el fet que per tenir la informació ben ordenada i evitar possibles errors, s'ha creat una classe que envolta (_Wrapper_) la classe `BestLapData`.

```csharp
[Serializable]
public class BestLapDataListWrapper
{
    public List<BestLapData> items;
}
```

D'aquesta manera podem empaquetar tota la informació en una sola llista i exportar-ho i importar-ho fàcilment en format `JSON`.
Per poder eliminar totes les dades enregistrades i començar la partida de nou es cerquen tots els arxius creats que emmagatzemen informació de la partida gravada i, si existeixen, els elimina.

```csharp
public static void DeleteData()
{
    string filePath = Application.persistentDataPath + "/best_circuit_time_summary.json";
    if (File.Exists(filePath))
    {
        File.Delete(filePath);
        Debug.Log($"Save file deleted at {filePath}");
    }
    else
    {
        Debug.LogWarning($"Save file not found at {filePath}");
    }

    string[] ghostFiles = Directory.GetFiles(Application.persistentDataPath, "ghost_best_lap_circuit_*.json");
    if (ghostFiles.Length == 0)
    {
        Debug.LogWarning("No ghost lap files found to delete.");
        return;
    }
    foreach (string ghostFile in ghostFiles)
    {
        File.Delete(ghostFile);
        Debug.Log($"Deleted ghost lap file: {ghostFile}");
    }
}
```

### Tria del circuit

Per a la tria de circuit i posterior càrrega de l'escena, del fantasma de la millor volta i l'enregistrament de les dades s'ha decidit crear un `ScriptableObject` que permeti alhora emmagatzemar les dades i facilitar la comunicació d'aquesta als diferents elements del joc que han d'accedir a ella.

```csharp
public class CircuitInfo : ScriptableObject
{
    public int circuitID;
    public string circuitName;
    public string circuitScene;
    public Sprite circuitThumbnail;
}
```

D'aquesta manera, a cada botó se li assigna una classe que rep l'objecte `CircuitInfo` i l'objecte que mostra el _Thumbnail_ per poder actualitzar-la quan el cursor del ratolí passa per sobre. També s'actualitza el títol i el millor temps obtingut del circuit que s'està triant.

```csharp
public class CircuitButton : MonoBehaviour, IPointerEnterHandler
{
    public CircuitInfo circuitInfo;
    public ThumbnailDisplay thumbnail;
    [SerializeField] private TMP_Text circuitNameText;

    void Start()
    {
        circuitNameText.text = circuitInfo.circuitName;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        thumbnail.UpdateThumbnail(circuitInfo.circuitThumbnail);
        thumbnail.UpdateBestTime(circuitInfo.circuitID);
    }

    public void ChooseCircuit()
    {
        MainMenuManager.Instance.SetCircuit(circuitInfo);
    }
}
```

Per poder gestionar el hover, els botons han d'heredar d'`IPointerEnterHandler`.

### Tria de cotxe

<div align="center">
<img src="ReadmeImages/CarSelection.gif" width="600"> <br> <br>
</div>

Com es pot observar, s'ha implementat un indicador visual que compara els paràmetres clau de la conducció del cotxe triat amb el valor màxim de cada paràmetre. També es mostra un text en pantalla si el cotxe permet derrapar polsant el fre de mà.

Per poder gestionar aquesta informació s'ha creat una llista de tots els cotxes disponibles. Cada un té una classe que emmagatzema la informació clau per gestionar els canvis d'escena:

```csharp
public class CarInfo : MonoBehaviour
{
    public CinemachineCamera carCamera;
    public GameObject carPrefab;
    public string carName;
    ArcadeVehicleController carController;
    [HideInInspector] public float maxSpeed;
    [HideInInspector] public float acceleration;
    [HideInInspector] public float handling;
    [HideInInspector] public bool canDrift;

    void Start()
    {
        carController = carPrefab.GetComponent<ArcadeVehicleController>();
        maxSpeed = carController.MaxSpeed;
        acceleration = carController.accelaration;
        handling = carController.turn;
        canDrift = carController.kartLike;
    }
}
```

Quan el menú inicial es troba en selecció de cotxe i el jugador canvia de cotxe, aquest passa la nova informació al component que gestiona la mostra en pantalla de les barres. Perquè l'actualització sigui fluida i d'aspecte professional s'ha implementat una breu animació amb una corutina.

```csharp
private IEnumerator UpdateStatsCoroutine(CarInfo car)
{
  float elapsedTime = 0f;
  float duration = 0.5f;

  float initialSpeedFill = speedBar.fillAmount;
  float targetSpeedFill = car.maxSpeed / maxSpeed;

  float initialAccFill = accBar.fillAmount;
  float targetAccFill = car.acceleration / maxAcc;

  float initialHandlingFill = handlingBar.fillAmount;
  float targetHandlingFill = car.handling / maxHandling;

  while (elapsedTime < duration)
  {
      elapsedTime += Time.deltaTime;
      float t = Mathf.Clamp01(elapsedTime / duration);

      speedBar.fillAmount = Mathf.Lerp(initialSpeedFill, targetSpeedFill, t);
      accBar.fillAmount = Mathf.Lerp(initialAccFill, targetAccFill, t);
      handlingBar.fillAmount = Mathf.Lerp(initialHandlingFill, targetHandlingFill, t);

      yield return null;
  }

  
  speedBar.fillAmount = targetSpeedFill;
  accBar.fillAmount = targetAccFill;
  handlingBar.fillAmount = targetHandlingFill;
}
```

Per gestionar les transicions entre vehicles es crea una classe que rep la càmera del cotxe que es vol visualitzar i transiciona cap a ell. Per fer-ho de forma suau, el cinemachine ha d'estar configurat com a *Default blend = Ease In Out*. Aquesta configuració s'ha realitzat directament sobre l'editor de ***Unity***.

Un cop seleccionat el cotxe es passa tota la informació necessària a un altre `ScriptableObject` que és qui passa la informació important entre escenes. Aquest passa informació com el cotxe que cal instanciar, i la ID del circuit per poder carregar les dades del moviment que ha de fer el cotxe fantasma.

## Durant la cursa
Durant la cursa hi ha diversos aspectes tècnics a comentar. S'ha decidit tenir un *HUD* net on només es mostra el temps de cada volta, la diferència de cada una amb el millor temps, i la velocitat a la que va el cotxe. També s'ha optat per un sistema de pausa que permet continuar la cursa, reintentar-la o tornar al menú inicial. En acabar el circuit, el menú s'obre automàticament i permet veure la repetició, reintentar o tornar al menú.

### Gestió de la pausa
Per gestionar la pausa sense que el món continuï viu i el cotxe es mogui per inèrcia, es fa servir el `Time.timeScale` que permet escalar en percentatge la freqüència de càlcul de l'`Update()` i del `FixedUpdate()`. A més, com el menú és diferent tant per la pausa com per a la finalització del circuit, es gestiona per codi quin botó es mostra en primer lloc, si reprendre o si veure la repetició.

### Gestió de la reproducció del fantasma de la millor volta
<div align="center">
<img src="ReadmeImages/CountdownGhostCar.gif" width="600">
<br> <br>
</div>

La gestió de la reproducció del cotxe fantasma té 3 punts clau:
* **Enregistrament:** Un script encarregat de registrar la posició, la rotació, l'input de gir i el temps de la mostra en un arxiu `ScriptableObject` que s'encarrega d'emmagatzemar les dades de la cursa actual.

```csharp
public class LapRecordingContainerSO : ScriptableObject
{
    public List<Vector3> carPositions = new List<Vector3>();
    public List<Quaternion> carRotations = new List<Quaternion>();
    public List<float> steeringInputs = new List<float>(); 
    public List<float> sampleTimes = new List<float>();
    public List<float> lapTimes = new List<float>();
    public int circuitIndex;
    public float totalTime = 0f;
    
    // ...

    public void AddNewData(Vector3 position, Quaternion rotation, float steering, float time)
    {
        carPositions.Add(position);
        carRotations.Add(rotation);
        steeringInputs.Add(steering);
        sampleTimes.Add(time);
    }
}
```

* **Reproducció:** Un script adjunt als objectes del cotxe rep un segon `ScriptableObject` amb la millor cursa i reprodueix el moviment realitzant originalment pel vehicle. Per coherència amb altres videojocs que basen la contrarellotge en 3 voltes, el fantasma reprodueix el circuit complet un cop s'ha superat, com a mínim, una vegada tot el recorregut.<br> 
Per evitar que la reproducció sigui un model flotant, s'aprofita que, tal i com es detalla posteriorment, el moviment del vehicle està simulat per una esfera que roda pel circuit. Es calcula segons la distancia recorreguda entre punts i el radi de l'esfera quin angle ha girat i aquest s'aplica a les pròpies rodes:

*Matemàtiques de la simulació del gir de rodes:*
```csharp
public void ForceWheelRotation(float distanceDiff, float steering)
{
    float rbRadius = rb.GetComponent<SphereCollider>().radius;
    float rotationAmount = distanceDiff / rbRadius * (360 / (2 * Mathf.PI));
    Vector3 newRotation = Quaternion.Euler(FW.localRotation.eulerAngles.x, 30 * steering, FW.localRotation.eulerAngles.z);
    foreach (Transform FW in FrontWheels)
    {
        FW.localRotation = Quaternion.Slerp(FW.localRotation, newRotation, 0.7f * Time.deltaTime / Time.fixedDeltaTime);
        FW.GetChild(0).Rotate(rotationAmount, 0, 0);
    }
    foreach (Transform RW in RearWheels)
    {
        RW.Rotate(rotationAmount, 0, 0);
    }
}
```

* **Guardar dades:** Si en acabar el circuit el temps total és inferior al millor, l'script s'encarrega de registrar-ho com a nova millor cursa i l'emmagatzema en un arxiu `Json`. Aquests arxius es generen un per cada circuit. A l'iniciar l'escena, es carrega a l'`ScriptableObject` del cotxe fantasma el millor temps llegint-lo des d'aquest arxiu:
```csharp
 public static void SaveBestLap(LapRecordingContainerSO lapData)
{
    BestLapGhostData ghostData = new BestLapGhostData();
    ghostData.CopyFrom(lapData);

    string jsonData = JsonUtility.ToJson(ghostData, true);
    string filePath = Application.persistentDataPath + $"/ghost_best_lap_circuit_{lapData.circuitIndex}.json";
    File.WriteAllText(filePath, jsonData);
    Debug.Log($"Best lap saved to {filePath}");
}

public static void LoadBestLap(int circuitIndex, LapRecordingContainerSO lapData)
{
    string filePath = Application.persistentDataPath + $"/ghost_best_lap_circuit_{circuitIndex}.json";

    if (File.Exists(filePath))
    {
        string jsonData = File.ReadAllText(filePath);
        BestLapGhostData ghostData = JsonUtility.FromJson<BestLapGhostData>(jsonData);

        lapData.CopyFrom(ghostData);
    }
    else
    {
        lapData.Reset();
        Debug.LogWarning($"Best lap file not found at {filePath}");
    }
}
```

### Sistema de reproducció de la repetició
S'aprofita l'enregistrament de la cursa realitzada pel jugador i al propi cotxe s'inhabilita el càlcul de físiques per poder forçar el moviment a mode de reproducció de la repetició. Per generar un tipus de reproducció similar a l'estàndar de la indústria, s'ha fet servir cinemachine. En aquest cas el *Default Blend* es configura com a *Cut* per fer canvis secs entre càmeres. Un script s'encarrega de gestionar les prioritats dins de les càmeres disponibles de seguiment del cotxe:

```csharp
public class CameraManagement : MonoBehaviour
{
    [SerializeField] private float timeBetweenCameraSwitches = 5f;
    List<CinemachineCamera> cameras = new List<CinemachineCamera>();
    private CinemachineCamera currentCamera, defaultCamera;
    Coroutine activeCoroutine;


    public static CameraManagement Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCamera(CinemachineCamera newCamera)
    {
        if (currentCamera != null)
        {
            currentCamera.Priority = 0;
        }
        newCamera.Priority = 10;
        currentCamera = newCamera;
    }

    public void SetDefaultCamera()
    {
        if (defaultCamera != null)
        {
            SetCamera(defaultCamera);
        }
        else
        {
            Debug.LogError("Default camera not set!");
        }
    }

    public void RegisterCameras(GameObject car)
    {
        cameras.Clear();
        CinemachineCamera[] foundCameras = car.GetComponentsInChildren<CinemachineCamera>();
        cameras.AddRange(foundCameras);
        foreach (var cam in cameras)
        {
            if (cam.tag == "DefaultCamera")
            {
                defaultCamera = cam;
                SetCamera(defaultCamera);
                break;
            }
        }

        if (cameras.Count == 0)
        {
            Debug.LogError("No cameras found in the player car prefab!");
        }
    }

    public void StartReplay()
    {
        SetDefaultCamera();
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        activeCoroutine = StartCoroutine(ReplayCameraSequence());
    }

    IEnumerator ReplayCameraSequence()
    {
        while (true)
        {
            int randomIndex = Random.Range(0, cameras.Count);
            SetCamera(cameras[randomIndex]);
            yield return new WaitForSeconds(timeBetweenCameraSwitches);
        }

    }

    public void StopReplay()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        SetDefaultCamera();
    }
}
```

L'script cerca el tag *DefaultCamera* per poder tenir la referència de la càmera en joc. Per evitar errors s'ha afegit un `Debug.LogError` per tal d'advertir si al prefab del cotxe manca l'etiqueta.

### Interfície HUD
<div align="center">
<img src="ReadmeImages/HUD.png" width="600">
<br><br>
</div>

A la interfície es mostra la velocitat del vehicle de forma numèrica i de forma gràfica en relació a la velocitat màxima assolible. Per fer-ho es treballa amb dos `Image`, la base i la del degradat de color que es pinta amb la propietat `fillAmount` de ***Unity***.
```csharp
public class ShowingSpeed : MonoBehaviour
{
    ArcadeVehicleController carController;
    [SerializeField] private Image filledSpeedMeter;
    [SerializeField] TMP_Text speedText;
    [Range (1f, 6f)] [SerializeField] private float showVelMultiplier = 1f;
    float currentSpeed = 0f;
    float maxSpeed;
    float percentageOfMaxSpeed;

    public void SetCarController()
    {
        carController = CircuitManager.Instance.playerCarController;
        maxSpeed = carController.MaxSpeed;
    }
    void Update()
    {
        currentSpeed = carController.GetCurrentSpeed();
        percentageOfMaxSpeed = currentSpeed / maxSpeed;
        filledSpeedMeter.fillAmount = percentageOfMaxSpeed;
        int showingSpeed = Mathf.RoundToInt(currentSpeed * showVelMultiplier);
        speedText.text = showingSpeed.ToString();
    }
}
```

La propietat `showVelMultiplier` es fa servir per adaptar el valor numèric mostrar al que correspondria amb la realitat.

A la part superior esquerra apareix el temps que ha transcorregut de la volta actual, així com el que el jugador ha invertit a cada volta. Quan es completa una volta, també es mostra la comparativa amb el temps de la volta actual del temps del fantasma.

```csharp
public void UpdateLapTime(float lapTime)
{
    lapTimeText.text = "Lap " + currentLap + ": " + TimeFormatter.FormatTime(lapTime);
}

public void UpdateCompletedLaps(List<LapData> lapData)
{
    string completedLaps = "";

    if (lapData != null && lapData.Count > 0)
    {
        foreach (LapData lap in lapData)
        {
            completedLaps += "Lap " + lap.lapNumber + ": " + TimeFormatter.FormatTime(lap.lapTime);

            if (referenceLapTimes.Count != 0)
            {
                float lapDifference = lap.lapTime - referenceLapTimes[lap.lapNumber - 1];
                string differenceText = lapDifference >= 0 ? " (+" : " (-";
                differenceText += TimeFormatter.FormatDiffTime(Mathf.Abs(lapDifference));
                differenceText += ")";
                completedLaps += differenceText;
            }

            completedLaps += "\n";
        }
    }

    completedLapsText.text = completedLaps;
}
```

Per mostrar el temps amb un format agradable, s'ha creat dos mètodes que transformen el temps en segons segons els estàndars en automovilisme. Per fer-la accessible per qualsevol classe, s'ha optat per convertir-la en classe pública i estàtica:

```csharp
public static class TimeFormatter
{
    public static string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    public static string FormatDiffTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
        if (minutes == 0)
            return string.Format("{0:00}.{1:000}", seconds, milliseconds);
        else
            return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}
```

### Físiques del vehicle
Per a les físiques del vehicle s'ha fet servir l'asset ***Arcade Vehicle Physics*** d'***Ash Dev***. Aquest és un asset que simula el moviment de vehicles a l'estil de jocs arcade. Això simplifica les físiques, creant una conducció molt més apta per a tots els públics. S'ha optat per aquest tipus de moviment per facilitar la compleció del circuit als jugadors.<br>
Originalment aquest asset no suportava el `New Input System` de manera que s'ha adaptat el codi per permetre-ho. Així com la comprovació del *layer* sobre la qual transita per poder simular el comportament en cada tipus de paviment. 

```csharp
//...

void OnEnable()
{
    InputManager.onMoveInput += ProvideSteeringInput;
    InputManager.onAccelerationInput += ProvideAccelerationInput;
    InputManager.onBrakeAction += ProvideBrakeInput;
}

void OnDisable()
{
    InputManager.onMoveInput -= ProvideSteeringInput;
    InputManager.onAccelerationInput -= ProvideAccelerationInput;
    InputManager.onBrakeAction -= ProvideBrakeInput;
}

private void ProvideSteeringInput(float input)
{
    steeringInput = input;
}

private void ProvideAccelerationInput(float input)
{
    accelerationInput = input;
}

private void ProvideBrakeInput(bool input)
{
    brakeInput = input;
}

public float GetGroundTypeMultiplier()
{
    var direction = -transform.up;
    origin = rb.position + rb.GetComponent<SphereCollider>().radius * Vector3.up;
    var maxdistance = rb.GetComponent<SphereCollider>().radius + 0.2f;
    if (GroundCheck == groundCheck.rayCast)
    {
        if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface))
        {
            currentSurfaceType = surfaceType.drivable;
            return 1f;
        }
        else if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, dirtTrackSurface))
        {
            currentSurfaceType = surfaceType.dirt;
            return dirtTrackMultiplier;
        }
        else if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, outOfBoundsSurface))
        {
            currentSurfaceType = surfaceType.outOfBounds;
            return outOfBoundsMultiplier;
        }
        else
        {
            Debug.Log("Ground type not detected");
            return 1f;
        }
    }

    else if (GroundCheck == groundCheck.sphereCaste)
    {
        if (Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, drivableSurface))
        {
            currentSurfaceType = surfaceType.drivable;
            return 1f;

        }
        else if (Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, dirtTrackSurface))
        {
            currentSurfaceType = surfaceType.dirt;
            return dirtTrackMultiplier;

        }
        else if (Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, outOfBoundsSurface))
        {
            currentSurfaceType = surfaceType.outOfBounds;
            return outOfBoundsMultiplier;

        }
        else
        {
            Debug.Log("Ground type not detected");
            return 1f;
        }
    }
    else
    {
        Debug.Log("Ground Check type not defined");
        return 1f;
    }
}
```

*NOTA: El dia 23 de Març, amb les modificacions ja fetes, el desenvolupador va llençar la versió adaptada a l'Input system actual. S'ha optat per mantenir la opció customitzada*

### Efectes de desperfectes al vehicle
<div align="center">
<img src="ReadmeImages/DamagedCarComparative.png" width="600">
<br><br>
</div>

Quan el vehicle col·lisiona amb un obstacle, si la velocitat excedeix el llindar es calcula i s'efectua una deformació en el punt d'impacte. El mètode emprat és la comparació dels punts de la malla respecte al punt de col·lisió i efectuar la deformació en una certa distància. Per poder ajustar-ho s'ha creat un script amb paràmetres configurables.<br>
Com aquest és un càlcul pesat, s'ha cercat la possibilitat de reduir l'impacte. La solució escollida és moure el càlcul en un altre fil per no provocar caigudes d'FPS. Per poder comunicar-se entre fils es fa servir una _flag_ `volatile bool`. És necessari fer-la `volatile` degut a que per funcionament dels fils hi ha risc de que no s'arribi a llegir mai el canvi en la bandera produït en el `Task.Run()`

```csharp
public class ImpactManaging : MonoBehaviour
{
    public ArcadeVehicleController vehicleController;
    public AudioClip impactClip;
    public AudioSource audioSource;
    public MeshFilter meshFilter;
    public Mesh mesh;
    public bool reset = false;

    [Header("Deformation Settings")]
    public float strength = 0.05f;
    public float maxDistance = 1f;

    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;
    private volatile bool hasResult;
    private Vector3[] resultVertices;

    [SerializeField] private float impactThreshold = 10f;

    void OnEnable()
    {
        if(CircuitManager.Instance != null)
        {
            CircuitManager.Instance.OnRestartCircuit += ResetDeformation;
        }
    }

    void OnDisable()
    {
        if(CircuitManager.Instance != null)
        {
            CircuitManager.Instance.OnRestartCircuit -= ResetDeformation;
        }
    }

    void Awake()
    {
        mesh = Instantiate(meshFilter.sharedMesh);
        meshFilter.sharedMesh = mesh;

        originalVertices = mesh.vertices;
        modifiedVertices = new Vector3[originalVertices.Length];
        Array.Copy(originalVertices, modifiedVertices, originalVertices.Length);
    }

    void Update()
    {
        if (reset)
        {
            Array.Copy(originalVertices, modifiedVertices, originalVertices.Length);
            ApplyToMesh(modifiedVertices);
            reset = false;
        }

        if (hasResult)
        {
            hasResult = false;
            modifiedVertices = resultVertices;
            ApplyToMesh(modifiedVertices);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (vehicleController.carVelocity.magnitude > impactThreshold)
        {
            if (audioSource != null && impactClip != null)
            {
                audioSource.PlayOneShot(impactClip);
            }
            vehicleController.Impacted();
            var contactPoint = collision.GetContact(0);
            DeformMeshAsync(contactPoint.point, -contactPoint.normal, strength, maxDistance);
        }
    }

    public void DeformMeshAsync(Vector3 impactPointWS, Vector3 impactNormalWS, float impactStrength, float impactMaxDistance)
    {

        Vector3 impactPointLS = transform.InverseTransformPoint(impactPointWS);
        Vector3 impactNormalLS = transform.InverseTransformDirection(impactNormalWS).normalized;

        Vector3[] baseVerts = new Vector3[modifiedVertices.Length];
        Array.Copy(modifiedVertices, baseVerts, modifiedVertices.Length);

        Task.Run(() =>
        {
            for (int i = 0; i < baseVerts.Length; i++)
            {
                float dist = Vector3.Distance(baseVerts[i], impactPointLS);
                
                //If it's too far from impact, skip it
                if (dist > impactMaxDistance) continue;

                float t = Mathf.Clamp01(dist / impactMaxDistance);
                float w = Mathf.SmoothStep(1f, 0f, t);
                
                // Modify vertex to simulate impact deformation
                baseVerts[i] -= impactNormalLS * (impactStrength * w);
            }

            resultVertices = baseVerts;
            hasResult = true;
        });
    }

    void ApplyToMesh(Vector3[] vertices)
    {
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void ResetDeformation()
    {
        reset = true;
    }

}
```

A més es produeix una penalització al moviment del vehicle quan aquest queda deformat.

## Solucions d'àudio
Per poder reproduir els diferents sons s'ha creat un objecte `AudioManager` encarregat de gestionar aquestes tasques. Aquest es comunica amb els altres amb l'arquitectura *Singleton*. Tot i que no és una pràctica recomanada per la generació de dependències, és la forma més simple d'accedir a ell i s'ha decidit fer aquesta petita concessió tècnica. Per evitar mals funcionaments, les crides a mètodes es fan previa comprovació de l'existència de l'objecte.

Com que és un joc de cotxes s'ha decidit mantenir els circuits sense tema musical principal per evitar la saturació acústica i reservar la importància al so del cotxe.

### Música en loop al menú inicial
Al menú principal es fa servir una pista de duració determinada fragmentada en dos parts:
* **Introducció:** És la part de la cançó que es reprodueix a l'inici i que no volem que es torni a repetir per poder aconseguir un loop professional
* **Loop:** És la part que es repeteix contínuament fins que el jugador tria el circuit i la següent escena es carrega, aturant així la cançó.

El gran repte tècnic és coordinar la introducció i el loop perquè no es produeixi cap micro tall en l'àudio. Per solucionar-ho, quan es crida a reproduir una cançó, el codi comprova si es tracta d'un loop amb dos parts. Llavors es calcula la durada del clip inicial i, fent ús del temps d'alta precisió per àudios, es calcula quan ha de reproduir el següent clip:

*Crida de l'AudioManager*
```csharp
public void PlayMusic(AudioClip intro, AudioClip loop)
{
    StopMusic();
    musicPlayer.PlayMusic(intro, false);
    if(loop != null)
    {
        float delay = intro != null ? intro.length : 0f;
        loopPlayer.PlayMusic(loop, true, delay);
    }
}
```

*Gestió del reproductor d'audio*
```csharp
public void PlayMusic(AudioClip clip, bool loop, float delay = 0f)
{
    StopAllCoroutines();
    audioSource.Stop();
    audioSource.clip = clip;
    audioSource.loop = loop;
    audioSource.PlayScheduled(AudioSettings.dspTime + 0.1 + delay);
}
```

A més, per evitar talls bruscos en la transició de música a silenci, s'ha implementat un *fade out* del volum:
```csharp
public void StopMusic()
{
    if(audioSource.isPlaying)
    {
        StartCoroutine(StopMusicCoroutine());
    }
}

IEnumerator StopMusicCoroutine()
{
    float startVolume = audioSource.volume;
    while (audioSource.volume > 0.1f)
    {
        audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, fadeOutIntensity * Time.deltaTime);
        yield return null;
    }

    audioSource.Stop();
    audioSource.volume = startVolume; 
}
```

### SFX als botons
Tots els botons tenen el component d'event trigger que gestionen el hover per cridar a l'`AudioManager` que reprodueixi un so. En prémer qualsevol botó s'executa la lògica del botó i es fa una crida a l'`AudioManager`per reproduir el so d'acceptar.
```csharp
public void PlaySFX(AudioClip clip)
{
    audioSource.PlayOneShot(clip);
}
```

# Conclusions
En aquest projecte han aparegut molts reptes tècnics a nivell de programació i d'arquitectura de codi. En tot moment s'ha prioritzat la netedat i claredat del codi. Idealment s'ha intentat mantenir el *Single Responsibility Principle (SRP)* tot i que en algun cas, tenint en compte que és un projecte que no ha d'escalar, s'ha fet alguna concessió per tal de poder agilitzar el procés de producció.