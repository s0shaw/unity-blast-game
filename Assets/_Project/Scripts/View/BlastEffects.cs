using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PrimeTween;

namespace GemBlast.View
{
    /// <summary>
    /// Pooled, code-only blast effects: small sprite shards that fly out of
    /// each blasted block and a floating score popup. No scene setup needed.
    /// </summary>
    public class BlastEffects
    {
        private const int ShardsPerBlock = 4;
        private const int ShardSortingOrder = 100;

        private readonly Transform _root;
        private readonly Queue<SpriteRenderer> _shardPool = new Queue<SpriteRenderer>();
        private readonly Queue<TextMeshPro> _textPool = new Queue<TextMeshPro>();

        public BlastEffects(Transform root)
        {
            _root = root;
        }

        public void SpawnShards(Vector3 worldPos, Sprite sprite, float strength)
        {
            for (int i = 0; i < ShardsPerBlock; i++)
            {
                SpriteRenderer shard = GetShard();
                shard.sprite = sprite;
                shard.color = Color.white;
                shard.transform.position = worldPos;
                shard.transform.localScale = Vector3.one * 0.3f;

                Vector2 dir = Random.insideUnitCircle.normalized;
                Vector3 target = worldPos + (Vector3)(dir * Random.Range(0.6f, 1.2f) * strength)
                                          + Vector3.down * 0.5f;
                float duration = Random.Range(0.35f, 0.55f);

                Tween.Position(shard.transform, target, duration, Ease.OutQuad);
                Tween.Scale(shard.transform, Vector3.zero, duration, Ease.InQuad);
                Tween.Alpha(shard, 0f, duration)
                     .OnComplete(shard, Release);
            }
        }

        public void ShowScorePopup(Vector3 worldPos, int points, bool big)
        {
            TextMeshPro text = GetText();
            text.text = $"+{points}";
            text.fontSize = big ? 7f : 5f;
            text.color = big ? new Color(1f, 0.85f, 0.2f) : Color.white;
            text.transform.position = worldPos;
            text.transform.localScale = Vector3.one * 0.5f;

            Sequence.Create()
                .Group(Tween.Scale(text.transform, Vector3.one, 0.2f, Ease.OutBack))
                .Group(Tween.PositionY(text.transform, worldPos.y + 1.2f, 0.8f, Ease.OutQuad))
                .Group(Tween.Alpha(text, 0f, 0.4f, Ease.InQuad, startDelay: 0.4f))
                .OnComplete(text, ReleaseText);
        }

        private SpriteRenderer GetShard()
        {
            if (_shardPool.Count > 0)
            {
                var pooled = _shardPool.Dequeue();
                pooled.gameObject.SetActive(true);
                return pooled;
            }

            var go = new GameObject("Shard");
            go.transform.SetParent(_root, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = ShardSortingOrder;
            return sr;
        }

        private void Release(SpriteRenderer shard)
        {
            shard.gameObject.SetActive(false);
            _shardPool.Enqueue(shard);
        }

        private TextMeshPro GetText()
        {
            if (_textPool.Count > 0)
            {
                var pooled = _textPool.Dequeue();
                pooled.gameObject.SetActive(true);
                return pooled;
            }

            var go = new GameObject("ScorePopup");
            go.transform.SetParent(_root, false);
            var tmp = go.AddComponent<TextMeshPro>();
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.sortingOrder = ShardSortingOrder + 1;
            return tmp;
        }

        private void ReleaseText(TextMeshPro text)
        {
            text.gameObject.SetActive(false);
            _textPool.Enqueue(text);
        }
    }
}
