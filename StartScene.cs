using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : MonoBehaviour
{
    //Holds the all of the canvases
    [SerializeField] Canvas curr, manTTT, aiTTT;

    public void startManual()
    {
        //Disable current canvas
        curr.gameObject.SetActive(false);

        //Disable AI TTT canvas
        aiTTT.gameObject.SetActive(false);

        //Enable manual TTT canavs
        manTTT.gameObject.SetActive(true);

        //Start manual TTT game
        manTTT.GetComponent<TTT>().restart();
    }

    public void startRandomAI()
    {
        //Disable current canvas
        curr.gameObject.SetActive(false);

        //Disable man TTT canvas
        manTTT.gameObject.SetActive(false);

        //Enable AI TTT canavs
        aiTTT.gameObject.SetActive(true);

        //Play against the random AI
        aiTTT.GetComponent<AITTT>().smart = false;

        //Start AI TTT game
        aiTTT.GetComponent<AITTT>().restart();
    }

    public void startSmartAI()
    {
        //Disable current canvas
        curr.gameObject.SetActive(false);

        //Disable manual TTT canvas
        manTTT.gameObject.SetActive(false);

        //Enable AI TTT canavs
        aiTTT.gameObject.SetActive(true);

        //Play against the smart AI
        aiTTT.GetComponent<AITTT>().smart = true;

        //Start AI TTT game
        aiTTT.GetComponent<AITTT>().restart();
    }
}
