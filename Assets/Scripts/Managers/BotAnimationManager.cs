using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class BotAnimationManager : MonoBehaviour
{
    GameManager gm;
    AudioSource _audioSource;
    Animator _anim;
    LaunchVelocity _launchVelocity;
    ArrowMain _currentArrow;
    
    [SerializeField] Transform bowTransform;
    [SerializeField] Transform arrowSpawnPoint;
    enum State { Spawned, AimingStart, AimingFinished, Done}
    State _aState = State.Done;
    Vector3 _targetPos;
    Vector3 _vel;
    float _ikValue;
    
    Vector3 _offsetBow = new Vector3(0.06f, 0.08f, 0f);
    const int CONST_AimTime = 2;
    int _shoot = Animator.StringToHash("shoot");
    static int rdnIdle = Animator.StringToHash("rdnIdle");

    public int arrowIndex = 1;
    void Awake()
    {
        gm = GameManager.Instance;
        _audioSource = GetComponent<AudioSource>();
        _anim = GetComponent<Animator>();
        _launchVelocity = new LaunchVelocity();
        SetNextIdleAnimation(_anim);
    }
    


    void OnAnimatorIK(int layerIndex)
    {
        _currentArrow = gm.arrowManager.arrowReal;
        if ((_aState == State.AimingStart || _aState == State.AimingFinished) && _currentArrow != null)
        {
            Vector3 offsetVector = new Vector3(_offsetBow.x * bowTransform.up.x,
                _offsetBow.y * bowTransform.up.y,
                _offsetBow.z * bowTransform.up.z);
            Vector3 pos = new Vector3(_currentArrow.tip.position.x + offsetVector.x,
                _currentArrow.tip.position.y + offsetVector.y,
                _currentArrow.tip.position.z + offsetVector.z);
            
            _anim.SetIKPosition(AvatarIKGoal.LeftHand, pos);
        }
        _anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, _ikValue);
    }

    void FixedUpdate()
    {
        switch (_aState)
        {
            case State.Spawned:
                gm.arrowManager.arrowReal.myTransform.SetPositionAndRotation(arrowSpawnPoint.position, arrowSpawnPoint.rotation);
                break;
            case State.AimingStart:
                _vel = _launchVelocity.Vel(arrowSpawnPoint.position, _targetPos, gm.windManager.gravityVector + gm.windManager.windVector);
                Quaternion targetRot = Quaternion.Slerp(arrowSpawnPoint.rotation, Quaternion.LookRotation(_vel.normalized), _ikValue);
                gm.arrowManager.arrowReal.myTransform.SetPositionAndRotation(arrowSpawnPoint.position, targetRot);
                _ikValue += Time.deltaTime / CONST_AimTime;
                if (_ikValue >= 1f) _anim.speed = 1;
                break;
            case State.AimingFinished:
                _ikValue -= Time.deltaTime / CONST_AimTime;
                if (_ikValue <= 0)
                {
                    
                    _aState = State.Done;
                }
                break;
        }
        _ikValue = Mathf.Clamp01(_ikValue);
    }

    public void BeginShootingSequence(Vector3 targetPos)
    {
        _targetPos = targetPos;
        _anim.SetTrigger(_shoot);
    }

    public void AE_SpawnArrow()
    {
        _aState = State.Spawned;
        gm.arrowManager.SpawnRealArrow(arrowSpawnPoint.position, arrowSpawnPoint.rotation, (byte)arrowIndex);
    }

    public void AE_PauseToAim()
    {
        _aState = State.AimingStart;
        _anim.speed = 0f;
    }

    public void AE_Shoot()
    {
        _aState = State.Done;
        gm.arrowManager.arrowReal.Release(_vel);
        AudioManager.Instance.PlayOnSpecificAudioSource(_audioSource, AudioManager.Instance.bowRelease);
        _aState = State.AimingFinished;
    }

    public static void SetNextIdleAnimation(Animator anim) => anim.SetInteger(rdnIdle, Random.Range(0, 12));
}
