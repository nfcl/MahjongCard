using Data;
using System.Collections;
using UnityEngine;

namespace GameSceneUI
{
    public class DesktopPlayerInfoItem : MonoBehaviour
    {
        private static Sprite[] fengIcons;
        private static Font[] resFonts;
        private static Material[] resMaterials;

        public SpriteRenderer fengIcon;
        public SpriteRenderer waitingBar;
        public TextMesh resText;
        public MeshRenderer resRender;

        private int res = 0;
        private float showDistanceEndTime = 0;
        private static float showDistanceTime = 5;
        private bool hasACoroutine = false;

        public void Awake()
        {
            fengIcons ??= Resources.LoadAll<Sprite>("Desktop/feng");
            resFonts ??= new Font[]
            {
                    Resources.Load<Font>("Font/Res/blue/score_blue"),
                    Resources.Load<Font>("Font/Res/red/score_red"),
                    Resources.Load<Font>("Font/Res/yellow/score_yellow")
            };
            resMaterials ??= new Material[]
            {
                    Resources.Load<Material>("Font/Res/blue/score_blue"),
                    Resources.Load<Material>("Font/Res/red/score_red"),
                    Resources.Load<Material>("Font/Res/yellow/score_yellow")
            };
        }

        public void SetWaitBarState(bool isEnable)
        {
            waitingBar.gameObject.SetActive(isEnable);
        }
        public void SetFeng(FengKind kind)
        {
            fengIcon.sprite = fengIcons[(int)kind];
        }
        public void SetResTextFont(int kind)
        {
            resText.font = resFonts[kind];
            resRender.material = resMaterials[kind];
        }
        public void ShowRes(int num)
        {
            SetResTextFont(2);
            res = num;
            resText.text = res.ToString();
        }
        public void ShowResDistance(int num)
        {
            showDistanceEndTime = Time.time + showDistanceTime;
            if (!hasACoroutine)
            {
                hasACoroutine = true;
                StartCoroutine(HideResDistance());
            }

            if(num > 0)
            {
                SetResTextFont(0);
                resText.text = $"+{num}";
            }
            else
            {
                SetResTextFont(1);
                if(num == 0)
                {
                    resText.text = "0";
                }
                else
                {
                    resText.text = $"-{num}";
                }
            }
        }
        private IEnumerator HideResDistance()
        {
            yield return new WaitUntil(() => Time.time < showDistanceEndTime);
            ShowRes(res);
            hasACoroutine = false;
        }
    }
}