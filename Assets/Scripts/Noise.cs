using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{

    [SerializeField] private GameObject crunch;
    private bool crunchStatus = false;
    [SerializeField] public AudioClip crunchSound;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>().playOnAwake = false;
        GetComponent<AudioSource>().clip = crunchSound;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Crunch()
    {
        crunchStatus = true;
        crunch.SetActive(true);
        Invoke("StopCrunching", 1f);
    }

    private void StopCrunching()
    {
        crunchStatus = false;
        crunch.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {


        if (other.tag == "Player")
        {
            Crunch();
            GetComponent<AudioSource>().Play();
        }

    }

    public bool GetCrunchStatus()
    {
        return crunchStatus;
    }
}
