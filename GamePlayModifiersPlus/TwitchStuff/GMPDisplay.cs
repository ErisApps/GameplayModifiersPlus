﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;
namespace GamePlayModifiersPlus.TwitchStuff
{
    public class GMPDisplay : MonoBehaviour
    {
        TextMeshProUGUI chargeText;
        TextMeshProUGUI chargeCountText;
        public TextMeshProUGUI cooldownText;
        public TextMeshProUGUI activeCommandText;
        Image chargeCounter;
        private void Awake()
        {
            Init();
        }





        void Init()
        {
            GameObject textObj = new GameObject("GMPDisplayText");
            if (ChatConfig.uiOnTop)
            {
                textObj.transform.position = new Vector3(0.1f, 3f, 7f);
                textObj.transform.localScale *= 1.5f;
            }
  
            else
            {
                textObj.transform.position = new Vector3(0.2f, -1f, 7f);
                textObj.transform.localScale *= 2.0f;
            }


            var counterImage = ReflectionUtil.GetPrivateField<Image>(
    Resources.FindObjectsOfTypeAll<ScoreMultiplierUIController>().First(), "_multiplierProgressImage");

            GameObject canvasobj = new GameObject("GMPDisplayCanvas");
            Canvas canvas = canvasobj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            CanvasScaler cs = canvasobj.AddComponent<CanvasScaler>();
            cs.scaleFactor = 10.0f;
            cs.dynamicPixelsPerUnit = 10f;
            GraphicRaycaster gr = canvasobj.AddComponent<GraphicRaycaster>();
            canvasobj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1f);
            canvasobj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);

            GameObject counter = new GameObject("GMPDisplayCounter");
            chargeCounter = counter.AddComponent<Image>();
            counter.transform.parent = canvasobj.transform;
            counter.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0.5f);
            counter.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.5f);
            counter.transform.localScale = new Vector3(1f, 1f, 1f);

            chargeCounter.sprite = counterImage.sprite;
            chargeCounter.type = Image.Type.Filled;
            chargeCounter.fillMethod = Image.FillMethod.Radial360;
            chargeCounter.fillOrigin = (int)Image.Origin360.Top;
            chargeCounter.fillClockwise = true;
            chargeCounter.fillAmount = Plugin.charges / ChatConfig.maxCharges;
            chargeCounter.color = Color.green;

            GameObject background = new GameObject("GMPDisplayBackGround");
            var bg = background.AddComponent<Image>();
            background.transform.parent = canvasobj.transform;
            background.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0.5f);
            background.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.5f);
            background.transform.localScale = new Vector3(1f, 1f, 1f);

            bg.sprite = counterImage.sprite;
            bg.CrossFadeAlpha(0.05f, 1f, false);

            canvasobj.GetComponent<RectTransform>().SetParent(textObj.transform, false);
            canvasobj.transform.localPosition = new Vector3(-0.1f, -.1f, 0f);

            chargeText = CustomUI.BeatSaber.BeatSaberUI.CreateText(canvas.transform as RectTransform, "Charges", new Vector2(-0.25f, 0.5f));
            chargeText.fontSize = 3;
            chargeText.transform.localScale *= .08f;
            chargeText.color = Color.white;
            //    chargeText.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            chargeText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1f);
            chargeText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);
            chargeText.GetComponent<RectTransform>().SetParent(canvas.transform, false);


            chargeCountText = CustomUI.BeatSaber.BeatSaberUI.CreateText(canvas.transform as RectTransform, Plugin.charges.ToString(), new Vector2(0, 0));
            chargeCountText.text = Plugin.charges.ToString();
            chargeCountText.alignment = TextAlignmentOptions.Center;
            chargeCountText.transform.localScale *= .08f;
            chargeCountText.fontSize = 2.5f;
            chargeCountText.color = Color.white;
            chargeCountText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1f);
            chargeCountText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);
            chargeCountText.GetComponent<RectTransform>().SetParent(canvas.transform, false);
         //   chargeCountText.transform.localPosition = new Vector3(-0.0925f, -.13f, 0f);

            cooldownText = CustomUI.BeatSaber.BeatSaberUI.CreateText(canvas.transform as RectTransform, Plugin.charges.ToString(), new Vector2(-1f, 0.015f));
            cooldownText.text = "";
            cooldownText.alignment = TextAlignmentOptions.MidlineRight;
            cooldownText.fontSize = 2.5f;
            cooldownText.transform.localScale *= .08f;
            cooldownText.color = Color.red;
       //     cooldownText.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            cooldownText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 10f);
            cooldownText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);
            cooldownText.GetComponent<RectTransform>().SetParent(canvas.transform, false);

            activeCommandText = CustomUI.BeatSaber.BeatSaberUI.CreateText(canvas.transform as RectTransform, Plugin.charges.ToString(), new Vector2(1f, 0.015f));
            activeCommandText.text = "";
            activeCommandText.alignment = TextAlignmentOptions.MidlineLeft;
            activeCommandText.fontSize = 2.5f;
            activeCommandText.transform.localScale *= .08f;
            activeCommandText.color = Color.yellow;
       //     activeCommandText.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            activeCommandText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 10f);
            activeCommandText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);
            activeCommandText.GetComponent<RectTransform>().SetParent(canvas.transform, false);
        }

        void Update()
        {
            chargeCounter.fillAmount = Mathf.Lerp(chargeCounter.fillAmount, (float)Plugin.charges / ChatConfig.maxCharges, .03f);
            chargeCountText.text = Plugin.charges.ToString();
        }

        public void Destroy()
        {
            Destroy(GameObject.Find("GMPDisplayCanvas"));
            Destroy(GameObject.Find("GMPDisplayCounter"));
            Destroy(GameObject.Find("GMPDisplayBackGround"));
            Destroy(GameObject.Find("GMPDisplayText"));
            Destroy(GameObject.Find("GMPDisplayCoolDown"));
            Destroy(GameObject.Find("GMPDisplayActiveCommands"));
        }
    }



}