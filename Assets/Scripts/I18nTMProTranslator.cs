using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class I18nTMProTranslator : MonoBehaviour
{
    void Start()
    {
        var fieldId = gameObject.name;
        var text = GetComponent<TMP_Text>();
        if (text != null)
            if (fieldId == "ISOCode")
                text.text = I18n.GetLanguage();
            else
                text.text = I18n.GetText(fieldId, text.text);
    }
}
