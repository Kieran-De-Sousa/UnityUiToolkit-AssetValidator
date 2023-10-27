using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class s_Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float playerMovementSpeed = 1.0f;

    private Rigidbody m_rigidbody;

    private Input_player m_inputActions;
    private Vector3 m_moveVector;

    private void Awake()
    {
        m_inputActions = new Input_player();
    }

    // Start is called before the first frame update
    void Start()
    {
        OnEnable();

        m_rigidbody = GetComponent<Rigidbody>();
    }

    
    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        m_moveVector = m_inputActions.gameplay.Move.ReadValue<Vector3>();
        m_rigidbody.velocity = new Vector3(m_moveVector.x * playerMovementSpeed, m_rigidbody.velocity.y, m_moveVector.z * playerMovementSpeed);
    }

    private void OnEnable()
    {
        m_inputActions.gameplay.Enable();
    }

    private void OnDisable()
    {
        m_inputActions.gameplay.Disable();
    }
}
