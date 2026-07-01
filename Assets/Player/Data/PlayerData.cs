using UnityEngine;

[CreateAssetMenu (fileName = "newPlayerStats", menuName = "Data/Player")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private float _maxHealth;
    public float MaxHealth => _maxHealth;
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed => _moveSpeed;
    [SerializeField] private float _jumpForce;
    public float JumpForce => _jumpForce;
    [SerializeField] private float _sprintSpeedMultiplier;
    public float SprintSpeedMultiplier => _sprintSpeedMultiplier;
    [SerializeField] private float _stamina;
    public float Stamina => _stamina;
}