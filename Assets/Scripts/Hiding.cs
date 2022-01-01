using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hiding : MonoBehaviour
{

    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject closed;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private UnityEngine.UI.Text hidePrompt;

    bool canHide = false;
    bool hidden = false;

    // Start is called before the first frame update
    void Start()
    {
        //MeshFilter mesh = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canHide && Input.GetKeyDown(KeyCode.E))
        {
            if (!hidden)
            {
                Player.SetActive(false);
                hidden = true;
                mesh.enabled = false;
                closed.SetActive(true);
                hidePrompt.text = "Press E to stop hiding";
            }
            else
            {
                Player.SetActive(true);
                hidden = false;
                mesh.enabled = true;
                closed.SetActive(false);
                hidePrompt.text = "Press E to hide";
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            canHide = true;
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canHide = false;
        }
    }

    public bool GetHidenStatus()
    {
        return hidden;
    }

}
