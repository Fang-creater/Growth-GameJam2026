using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SnExtension;
using UnityEngine;

namespace Regrowth
{
    public class Player : BehaviourSingleton<Player>
    {
        private static readonly int Climbing = Animator.StringToHash("Climbing");
        private static readonly int HSpeed = Animator.StringToHash("HSpeed");
        private static readonly int Sleeping = Animator.StringToHash("Success");
        private static readonly int Defeat = Animator.StringToHash("Defeat");

        [Header("Movement Settings")]
        [SerializeField] private float maxSpeed = 8f;
        [SerializeField] private float acceleration = 50f;
        [SerializeField] private float gravityScale = 3.2f;
        [Header("Others")] 
        [SerializeField] private float climbTime = 1f;
        [SerializeField] private Transform groundCheck;

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
            if (_locked) return;
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            if (Input.GetButtonDown("Interact") && !_isClimbing && CheckOnGround())
                Climb(_onTree);
            if (Mathf.Abs(_horizontalInput) > 0.01)
            {
                var dir = _horizontalInput > 0 ? 1 : -1;
                transform.localScale = new Vector3(dir, transform.localScale.y, transform.localScale.z);
            }
            _animator.SetFloat(HSpeed, _horizontalInput);
        }
        private void FixedUpdate()
        {
            if (_locked) return;
            Move();
            if (CheckOnWater()) Defeated();
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
            if (tree == null) return;
            _isClimbing = true;
            _coll.enabled = false;
            var target = new Vector2(transform.position.x, tree.GetTargetPos(transform.position).y);
            var direction = target.y > transform.position.y ? 1 : -1;
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
        private bool CheckOnGround()
        {
            return Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Ground", "TreeCrown"));
        }
        private bool CheckOnWater()
        {
            return Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Water"));
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
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Danger"))
            {
                Defeated().Forget();
            }
        }

        public async UniTaskVoid Sleep()
        {
            _locked = true;
            _animator.SetTrigger(Sleeping);
            _rb.bodyType = RigidbodyType2D.Static;
            await UniTask.WaitForSeconds(1);
            //NextScene
        }
        private async UniTaskVoid Defeated()
        {
            _locked = true;
            _animator.SetTrigger(Defeat);
            _rb.bodyType = RigidbodyType2D.Static;
            await UniTask.WaitForSeconds(1);
            SnSceneManager.ReloadCurrentScene();
        }
    }
}