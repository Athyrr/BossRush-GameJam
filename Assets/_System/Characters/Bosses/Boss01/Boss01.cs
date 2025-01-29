using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Boss01 : Boss
{
    private Animator _anim;
    [HideInInspector] public AnimatorStateInfo animState; //need proper mode detection

    [SerializeField] private float _morphInterval = 10f;

    public int PlayerHit = 0;
    public int MorphFailure = 0;
    [SerializeField] private float _range;
    [SerializeField] private float _rangeMult = 5;

    [SerializeField] private GameObject turret;
    public bool CanShoot = false;


    private void Start()
    {
        _anim = GetComponent<Animator>();

        InvokeRepeating("MorphTry", _morphInterval, _morphInterval);
    }

    private void MorphTry()
    {
        float morphChance = PlayerHit * MorphFailure;

        if (animState.IsName("Ball") && Vector3.Distance(transform.position, Target.position) > _range || animState.IsName("Turret") && Vector3.Distance(transform.position, Target.position) < _range)
            morphChance *= _rangeMult;

        morphChance = Mathf.Clamp(morphChance, 1, 101);
        Debug.Log("morphChance = " + morphChance);
        var r = Random.Range(1, 101);
        Debug.Log("random = " + r);

        if (r < morphChance && (IsGrounded(true) || animState.IsName("Turret")))
        {
            PlayerHit = 0; MorphFailure = 0;
            _anim.SetTrigger("trMorph");
        }
        else
            MorphFailure += 1;
    }

    private void OnMorph()
    {
        transform.GetComponent<Rigidbody>().isKinematic = true;
        transform.GetComponent<Boss01Mvt>().enabled = false;
        turret.SetActive(false);
        CanShoot = false;

    }
    private void BallMorph()
    {
        transform.GetComponent<Rigidbody>().isKinematic = false;
        transform.GetComponent<Boss01Mvt>().enabled = true;
    }

    private void TurretMorph()
    {
        turret.SetActive(true);
        CanShoot = true;

        transform.up = GetGroundNormal(true);
        turret.transform.position = transform.position + transform.up * 2;
    }

    //debug
    private void Update()
    {
        animState = _anim.GetCurrentAnimatorStateInfo(0);

        if (Input.GetKeyDown(KeyCode.Space))
            _anim.SetTrigger("trMorph");
    }
}
