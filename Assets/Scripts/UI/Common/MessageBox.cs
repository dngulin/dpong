using DPong.UI.Holder;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.UI.Common
{
    public class MessageBox : UIHolderWrapper
    {
        [SerializeField] private Text _titleLabel;
        [SerializeField] private Text _messageLabel;
        [SerializeField] private Text _buttonLabel;

        [SerializeField] private Button _button;

        internal void Init(string title, string message, string buttonText)
        {
            _titleLabel.text = title;
            _messageLabel.text = message;
            _buttonLabel.text = buttonText;

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(Hide);
        }
    }
}