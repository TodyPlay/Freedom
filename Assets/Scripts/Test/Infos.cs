using System.Collections;
using TMPro;
using UnityEngine;

namespace Test
{
    //显示一些系统信息，比如帧率之类的
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class Infos : MonoBehaviour
    {
        private TextMeshProUGUI m_text;

        private void Awake()
        {
            m_text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            StartCoroutine(UpdateInfo());
        }

        IEnumerator UpdateInfo()
        {
            while (true)
            {
                var fps = 1f / Time.deltaTime;

                m_text.text = @$"fps:{fps}";
                yield return new WaitForSeconds(1);
            }
        }
    }
}