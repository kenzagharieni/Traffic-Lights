using System.Collections;
using UnityEngine;

[System.Serializable]
public class TrafficLightSet
{
    public SpriteRenderer redCircle;
    public SpriteRenderer yellowCircle;
    public SpriteRenderer greenCircle;

    private Color offColor = new Color(0.2f, 0.2f, 0.2f);

    public void TurnRed()
    {
        redCircle.color = Color.red;
        yellowCircle.color = offColor;
        greenCircle.color = offColor;
    }

    public void TurnYellow()
    {
        redCircle.color = offColor;
        yellowCircle.color = Color.yellow;
        greenCircle.color = offColor;
    }

    public void TurnGreen()
    {
        redCircle.color = offColor;
        yellowCircle.color = offColor;
        greenCircle.color = Color.green;
    }
}

public class TrafficLightFSM : MonoBehaviour
{
    // Added new Clearance states for the Scenario 2 all-stop option
    public enum TrafficState { Phase1, Phase2, Phase3, Phase4, Phase5, Clearance1, Clearance2 }
    public TrafficState currentState = TrafficState.Phase1;

    [Header("North & South Lights")]
    public TrafficLightSet northLight;
    public TrafficLightSet southLight;

    [Header("East & West Lights")]
    public TrafficLightSet eastLight;
    public TrafficLightSet westLight;

    [Header("Scenario Settings")]
    [Tooltip("Check this to use Scenario 2: Adds an all-red stop before the traffic changes direction.")]
    public bool useAllStopClearance = false;

    [Header("Phase Durations (Seconds)")]
    public float greenLightTime = 4f;
    public float yellowLightTime = 1.5f;
    public float allRedTime = 2f;      // Used for Scenario 1's Phase 5
    public float clearanceTime = 1f;   // Short period for Scenario 2's all-stop

    void Start()
    {
        StartCoroutine(RunFSM());
    }

    IEnumerator RunFSM()
    {
        while (true)
        {
            switch (currentState)
            {
                case TrafficState.Phase1:
                    Debug.Log("Phase 1: North-South = Vert, East-West = Rouge");
                    SetNorthSouth(1); // Green
                    SetEastWest(0);   // Red
                    yield return new WaitForSeconds(greenLightTime);
                    currentState = TrafficState.Phase2;
                    break;

                case TrafficState.Phase2:
                    Debug.Log("Phase 2: North-South = Jaune, East-West = Rouge");
                    SetNorthSouth(2); // Yellow
                    SetEastWest(0);
                    yield return new WaitForSeconds(yellowLightTime);

                    // Branching logic based on your option
                    if (useAllStopClearance)
                        currentState = TrafficState.Clearance1;
                    else
                        currentState = TrafficState.Phase3;
                    break;

                case TrafficState.Clearance1:
                    Debug.Log("Clearance: TOUT ROUGE (Intersection clearing)");
                    SetNorthSouth(0); // Red
                    SetEastWest(0);   // Red
                    yield return new WaitForSeconds(clearanceTime);
                    currentState = TrafficState.Phase3;
                    break;

                case TrafficState.Phase3:
                    Debug.Log("Phase 3: North-South = Rouge, East-West = Vert");
                    SetNorthSouth(0);
                    SetEastWest(1); // Green
                    yield return new WaitForSeconds(greenLightTime);
                    currentState = TrafficState.Phase4;
                    break;

                case TrafficState.Phase4:
                    Debug.Log("Phase 4: North-South = Rouge, East-West = Jaune");
                    SetNorthSouth(0);
                    SetEastWest(2); // Yellow
                    yield return new WaitForSeconds(yellowLightTime);

                    // Branching logic based on your option
                    if (useAllStopClearance)
                        currentState = TrafficState.Clearance2;
                    else
                        currentState = TrafficState.Phase5;
                    break;

                case TrafficState.Clearance2:
                    Debug.Log("Clearance: TOUT ROUGE (Intersection clearing)");
                    SetNorthSouth(0); // Red
                    SetEastWest(0);   // Red
                    yield return new WaitForSeconds(clearanceTime);
                    currentState = TrafficState.Phase1; // Skip Phase 5 and loop back
                    break;

                case TrafficState.Phase5:
                    // Only used in Scenario 1
                    Debug.Log("Phase 5: North-South = Rouge, East-West = Rouge");
                    SetNorthSouth(0);
                    SetEastWest(0);
                    yield return new WaitForSeconds(allRedTime);
                    currentState = TrafficState.Phase1;
                    break;
            }
        }
    }

    void SetNorthSouth(int state)
    {
        if (state == 0) { northLight.TurnRed(); southLight.TurnRed(); }
        else if (state == 1) { northLight.TurnGreen(); southLight.TurnGreen(); }
        else if (state == 2) { northLight.TurnYellow(); southLight.TurnYellow(); }
    }

    void SetEastWest(int state)
    {
        if (state == 0) { eastLight.TurnRed(); westLight.TurnRed(); }
        else if (state == 1) { eastLight.TurnGreen(); westLight.TurnGreen(); }
        else if (state == 2) { eastLight.TurnYellow(); westLight.TurnYellow(); }
    }
}