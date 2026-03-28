using System;
using DG.Tweening;
using SnExtension;
using UnityEngine;

namespace Regrowth
{
    public class Player : BehaviourSingleton<Player>
    {
        private static readonly int Climbing = Animator.StringToHash("Climbing");
        private static readonly int HSpeed = Animator.StringToHash("HSpeed");
        private static readonly int VSpeed = Animator.StringToHash("VSpeed");
        private static readonly int Sleeping = Animator.StringToHash("Success");

        [Header("Movement Settings")]
        [SerializeField] private float maxSpeed = 8f;
        [SerializeField] private float acceleration = 50f;
        [SerializeField] private float gravityScale = 3.2f;
        [Header("Others")] 
        [SerializeField] private float climbTime = 0.5f;

        private Rigidbody2D _rb;
        private Collider2D _coll;
        private Animator _animator;
        private Tree _onTree = null;
        private float _horizontalInput;
        private bool _isClimbing = false, _locked = false;

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collider2D>();
            _animator = GetComponent<Animator>();
            _rb.gravityScale = gravityScale;
        }
        private void Update()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            if (Input.GetButtonDown("Interact") && !_isClimbing)
                Climb(_onTree);
        }
        private void FixedUpdate()
        {
            if (_locked) return;
            Move();
            _animator.SetFloat(HSpeed, _rb.velocity.x);
            _animator.SetFloat(VSpeed, _rb.velocity.y);
        }

        private void Move()
        {
            if (_isClimbing) return;
            float targetSpeed = _horizontalInput * maxSpeed;
            float velocity = Mathf.MoveTowards(_rb.velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
            _rb.velocity = new Vector2(velocity, _rb.velocity.y);
        }
        private void Climb(Tree tree)
        {
            if (tree) return;
            _isClimbing = true;
            _coll.enabled = false;
            var target = new Vector2(transform.position.x, tree.GetTargetPos(transform.position).y);
            var direction = (int)Mathf.Sign(target.x - transform.position.x);
            _animator.SetInteger(Climbing, direction);
            _rb.DOMove(target, climbTime)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _isClimbing = false;
                    _animator.SetInteger(Climbing, 0);
                    _coll.enabled = true;
                });
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Tree"))
            {
                _onTree = other.gameObject.GetComponent<Tree>();
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Tree"))
            {
                _onTree = null;
            }
        }

        public void Sleep()
        {
            _locked = true;
            _animator.SetTrigger(Sleeping);
            _rb.bodyType = RigidbodyType2D.Static;
        }
    }
}