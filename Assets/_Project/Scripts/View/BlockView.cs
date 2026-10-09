using UnityEngine;
using GemBlast.Core;
using PrimeTween;

namespace GemBlast.View
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BlockView : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private BlockData _data;
        private Tween _punchTween;

        public BlockData Data => _data;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(BlockData data, Sprite sprite, int sortingOrder)
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();

            _data = data;
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.sortingOrder = sortingOrder;
            
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;

            gameObject.name = $"Block_{data.Id}"; 
        }

        public void UpdateData(BlockData data)
        {
            _data = data;
        }

        public void UpdateVisuals(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public void MoveToProperties(int newX, int newY, Vector3 targetLocalPos)
        {
            Tween.StopAll(transform);
            Tween.LocalPosition(transform, targetLocalPos, 0.3f, Ease.InQuad);
        }

        public void AnimatePunch()
        {
            if (_punchTween.isAlive) return;

            _punchTween = Tween.ShakeLocalPosition(transform, 
                strength: new Vector3(0.15f, 0, 0), 
                duration: 0.3f, 
                frequency: 10);
        }

        public Sequence AnimateBlast(bool isBig = false)
        {
            Tween.StopAll(transform);
            var seq = Sequence.Create();
            
            float targetScale = isBig ? 1.5f : 1.2f;

            seq.Group(Tween.Scale(transform, Vector3.one * targetScale, 0.1f, Ease.OutQuad));
            
            if (isBig)
            {
                seq.Group(Tween.Rotation(transform, new Vector3(0, 0, Random.Range(-45f, 45f)), 0.1f));
            }

            seq.Chain(Tween.Scale(transform, Vector3.zero, 0.2f, Ease.InQuad));
            
            return seq;
        }
    }
}
