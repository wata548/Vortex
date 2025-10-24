/*using DG.Tweening;
using UnityEngine;

namespace Extension {
    public static class ExTween {

        public static Tween DOBreathing(this Transform pTransform, float pCycle, float pEndValue, Ease pEase = Ease.OutSine) =>
            pTransform.DOScale(pTransform.localScale * pEndValue, pCycle)
                .SetEase(pEase)
                .SetLoops(-1, LoopType.Yoyo);
    }
}*/