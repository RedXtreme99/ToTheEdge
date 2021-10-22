using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : MonoBehaviour
{
    // Movement and game mechanics
    [Header("Mechanics")]
    [SerializeField] float _moveSpeed = 12.0f;
    [SerializeField] float _turnSpeed = 3.0f;
    [SerializeField] float _shadeDuration = 0.5f;
    [SerializeField] float _solDuration = 2.0f;
    [SerializeField] GameObject _bullet = null;

    // References to ship parts to change color on powerup
    [Header("Ship Parts")]
    [SerializeField] GameObject _artToDisable = null;
    [SerializeField] GameObject _leftFin = null;
    [SerializeField] GameObject _rightFin = null;
    [SerializeField] GameObject _topFin = null;
    [SerializeField] GameObject _bulletSpawn = null;

    // Camera reference and vfx trails and particles
    [Header("Visual Feedback")]
    [SerializeField] Camera _camera = null;
    [SerializeField] FlashImage _flashImage = null;
    [SerializeField] TrailRenderer _trail = null;
    [SerializeField] ParticleSystem _shadeParticles = null;
    [SerializeField] ParticleSystem _solParticles = null;
    [SerializeField] ParticleSystem _deathParticles = null;
    [SerializeField] ParticleSystem _winParticles = null;

    // Materials to change ship part colors
    [Header("Materials")]
    [SerializeField] Material _mShade = null;
    [SerializeField] Material _mSol = null;
    [SerializeField] Material _mCharge = null;
    [SerializeField] Material _mAsteroid = null;
    [SerializeField] Material _mHazard = null;

    // Sound effects
    [Header("Sound Effects")]
    [SerializeField] AudioClip _shadeObtainSound = null;
    [SerializeField] AudioClip _shadeSound = null;
    [SerializeField] AudioClip _solObtainSound = null;
    [SerializeField] AudioClip _solSound = null;
    [SerializeField] AudioClip _chargeObtainSound = null;
    [SerializeField] AudioClip _chargeFireSound = null;
    [SerializeField] AudioClip _powerdownSound = null;
    [SerializeField] AudioClip _playerMoveSound = null;
    [SerializeField] AudioClip _playerDeathSound = null;
    [SerializeField] AudioClip _winSound = null;

    Rigidbody _rb = null;

    UIController _uiController = null;

    AudioSource[] _audioSources = null;

    // Store default background color to change back to when changed
    Color _defaultBackgroundColor;

    // Check if powerups obtained
    bool _shadeObtained = false;
    bool _solObtained = false;
    bool _chargeObtained = false;

    // Check if currently powered up
    bool _shadeEmpowered = false;
    bool _solEmpowered = false;

    bool _isDead = false;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _uiController = FindObjectOfType<UIController>();
        _defaultBackgroundColor = _camera.backgroundColor;

        _audioSources = GetComponents<AudioSource>();
        _audioSources[1].clip = _playerMoveSound;
    }

    private void Update()
    {
        if(_isDead)
        {
            _trail.Clear();
            _audioSources[1].Stop();
            return;
        }
        // Q triggers shade powerup coroutine
        if(Input.GetAxisRaw("Vertical") != 0)
        {
            if(!_audioSources[1].isPlaying)
            {
                _audioSources[1].Play();
            }
        }
        else
        {
            _audioSources[1].Stop();
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(_shadeObtained && !_shadeEmpowered)
            {
                StartCoroutine(ShadeSequence());
            }
        }
        // E triggers sol powerup coroutine
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(_solObtained && !_solEmpowered)
            {
                StartCoroutine(SolSequence());
            }
        }
        // Space fires bullet after charge obtained
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(_chargeObtained)
            {
                FireBullet();
            }
        }
    }

    // Physics based movement
    private void FixedUpdate()
    {
        if(_isDead)
        {
            return;
        }
        MoveShip();
        TurnShip();
    }

    // Move ship forwards and backwards
    void MoveShip()
    {
        // S/Down is -1, W/Up is 1, else 0. Scaled by move speed
        float moveAmountThisFrame = Input.GetAxisRaw("Vertical") * _moveSpeed;

        // Movement vector in z direction
        Vector3 moveDirection = transform.forward * moveAmountThisFrame;

        _rb.AddForce(moveDirection);
    }

    // Turn ship, rotation around y-axis cw/ccw
    void TurnShip()
    {
        // A/Left is -1, D/Right is 1, else 0. Scaled by turn speed
        float turnAmountThisFrame = Input.GetAxisRaw("Horizontal") * _turnSpeed;

        Quaternion turnOffset = Quaternion.Euler(0, turnAmountThisFrame, 0);
        _rb.MoveRotation(_rb.rotation * turnOffset);
    }

    // Function played on entering win volume
    public void Win()
    {
        _winParticles.Play();
        _artToDisable.SetActive(false);
        _isDead = true;
        if(_uiController != null)
        {
            _uiController.ShowText("You win!");
        }
    }

    // Function played on entering kill volume
    public void Kill()
    {
        _deathParticles.Play();
        _artToDisable.SetActive(false);
        _audioSources[0].PlayOneShot(_playerDeathSound);
        _isDead = true;
        if(_uiController != null)
        {
            _uiController.ShowText("You died!\nPress backspace to restart.");
        }
    }

    // Toggle powerup boolean after picked up and trigger effects
    public void EnablePowerup(string element)
    {
        if(element == "shade")
        {
            _shadeObtained = true;
            if(_leftFin != null && _mShade != null)
            {
                _leftFin.GetComponent<MeshRenderer>().material = _mShade;
            }
            _audioSources[0].PlayOneShot(_shadeObtainSound, 1.0f);
            _flashImage.StartFlash(0.25f, 0.5f, _mShade.color);
            StartCoroutine(TextSequence("Obtained Shadestep!\nPress Q to phase through shadow blocks", 4.0f));
        }
        if(element == "sol")
        {
            _solObtained = true;
            if(_rightFin != null && _mSol != null)
            {
                _rightFin.GetComponent<MeshRenderer>().material = _mSol;
            }
            _audioSources[0].PlayOneShot(_solObtainSound, 1.0f);
            _flashImage.StartFlash(0.25f, 0.5f, _mSol.color);
            StartCoroutine(TextSequence("Obtained Flamestride!\nPress E to be protected from fire blocks", 4.0f));
        }
        if(element == "charge")
        {
            _chargeObtained = true;
            if(_topFin != null && _mCharge != null)
            {
                _topFin.GetComponent<MeshRenderer>().material = _mCharge;
            }
            _audioSources[0].PlayOneShot(_chargeObtainSound, 1.0f);
            _flashImage.StartFlash(0.25f, 0.5f, _mCharge.color);
            StartCoroutine(TextSequence("Obtained Chargeblaster!\nPress space to fire lasers that destroy blue barriers", 4.0f));
        }
    }

    // Shade coroutine to toggle powerup for duration
    IEnumerator ShadeSequence()
    {
        _shadeEmpowered = true;
        ActivateShadeStep();
        yield return new WaitForSeconds(_shadeDuration);
        DeactivateShadeStep();
        _shadeEmpowered = false;
    }

    void ActivateShadeStep()
    {
        // Get all blocks with ShadeBlock tag to turn off renderer and collider during powerup
        GameObject[] shadeBlocks = GameObject.FindGameObjectsWithTag("ShadeBlock");
        foreach(GameObject block in shadeBlocks)
        {
            block.GetComponent<Collider>().isTrigger = true;
        }

        // Play sfx
        _audioSources[0].PlayOneShot(_shadeSound, 1.0f);

        // Screen flash to show effect
        _flashImage.StartFlash(0.25f, 0.5f, _mShade.color);

        _shadeParticles.Play();
    }

    void DeactivateShadeStep()
    {
        // Reenable mesh renderer and collider on shade blocks after duration elapsed
        GameObject[] shadeBlocks = GameObject.FindGameObjectsWithTag("ShadeBlock");
        foreach(GameObject block in shadeBlocks)
        {
            block.GetComponent<Collider>().isTrigger = false;
        }

        // Play sfx
        _audioSources[0].PlayOneShot(_powerdownSound, 1.0f);

        // Screen flash to show power down
        _flashImage.StartFlash(0.25f, 0.5f, _defaultBackgroundColor);
    }

    // Sol coroutine to toggle powerup for duration
    IEnumerator SolSequence()
    {
        _solEmpowered = true;
        ActivateFlameStrider();
        yield return new WaitForSeconds(_solDuration);
        DeactivateFlameStrider();
        _solEmpowered = false;
    }

    void ActivateFlameStrider()
    {
        // Get all blocks with FireBlock tag and disable their trigger status,
        // preventing them from killing the player
        GameObject[] fireBlocks = GameObject.FindGameObjectsWithTag("FireBlock");
        foreach(GameObject block in fireBlocks)
        {
            block.GetComponent<Collider>().isTrigger = false;
            block.GetComponent<MeshRenderer>().material = _mAsteroid;
        }

        _audioSources[0].PlayOneShot(_solSound, 1.0f);
        _flashImage.StartFlash(0.25f, 0.5f, _mSol.color);
        _solParticles.Play();
    }

    void DeactivateFlameStrider()
    {
        // Reactivate disabled fire block triggers
        GameObject[] fireBlocks = GameObject.FindGameObjectsWithTag("FireBlock");
        foreach(GameObject block in fireBlocks)
        {
            block.GetComponent<Collider>().isTrigger = true;
            block.GetComponent<MeshRenderer>().material = _mHazard;
        }

        // Play sfx
        _audioSources[0].PlayOneShot(_powerdownSound, 1.0f);

        // Screen flash to show power down
        _flashImage.StartFlash(0.25f, 0.5f, _defaultBackgroundColor);
    }

    void FireBullet()
    {
        _audioSources[0].PlayOneShot(_chargeFireSound, 1.0f);
        GameObject bullet = Instantiate(_bullet, _bulletSpawn.transform.position, _bulletSpawn.transform.rotation) as GameObject;
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 100);
    }

    IEnumerator TextSequence(string text, float delay)
    {
        _uiController.ShowText(text);
        yield return new WaitForSeconds(delay);
        _uiController.HideText();
    }
}
