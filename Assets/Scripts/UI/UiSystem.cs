using System.Collections.Generic;
using DPong.Assets;
using DPong.Localization;
using DPong.UI.Common;
using DPong.UI.Holder;
using UnityEngine;
using UObj = UnityEngine.Object;

namespace DPong.UI {
  public class UiSystem {
    private readonly Transform _root;
    private readonly Dictionary<UILayer, Transform> _layers = new Dictionary<UILayer, Transform>();

    private readonly UISystemResources _resources;

    public UiSystem(AssetLoader assetLoader, Canvas canvas) {
      _root = canvas.transform;
      _resources = assetLoader.Load<UISystemResources>("Assets/Content/UI/UISystemResources.asset");

      foreach (var layer in new[] {UILayer.Background, UILayer.Windows, UILayer.Foreground})
        CreateLayer(layer);
    }

    private void CreateLayer(UILayer layer) {
      var go = new GameObject(layer.ToString(), typeof(RectTransform));

      var rt = go.GetComponent<RectTransform>();
      rt.SetParent(_root);

      rt.anchorMin = Vector2.zero;
      rt.anchorMax = Vector2.one;
      rt.offsetMin = Vector2.zero;
      rt.offsetMax = Vector2.zero;

      _layers.Add(layer, rt);
    }

    public T Instantiate<T>(T prefab, UILayer layer, bool visible) where T : MonoBehaviour, IUserInterface {
      var ui = UObj.Instantiate(prefab, _layers[layer]);
      ui.SetInitialVisibility(visible);
      return ui;
    }

    public T InstantiateWrapped<T>(UIHolder prefab, UILayer layer, bool visible) where T : UIHolderWrapper {
      var holderInstance = UObj.Instantiate(prefab, _layers[layer]);
      var wrapperInstance = holderInstance.ContentRoot.GetComponent<T>();

      wrapperInstance.WrapHolder(holderInstance);
      wrapperInstance.SetInitialVisibility(visible);

      return wrapperInstance;
    }

    public T InstantiateAndWrap<T>(UIHolder holderPrefab, T wrapperPrefab, UILayer layer, bool visible) where T : UIHolderWrapper {
      var holderInstance = UObj.Instantiate(holderPrefab, _layers[layer]);
      var wrapperInstance = UObj.Instantiate(wrapperPrefab, holderInstance.ContentRoot);

      wrapperInstance.WrapHolder(holderInstance);
      wrapperInstance.SetInitialVisibility(visible);

      return wrapperInstance;
    }

    public T InstantiateWindow<T>(WindowType winType, T wrapper, bool visible) where T : UIHolderWrapper {
      return InstantiateAndWrap(_resources[winType], wrapper, UILayer.Windows, visible);
    }

    public MessageBox CreateMessageBox(bool visible, string title, string message, string buttonText)
    {
      var box = InstantiateAndWrap(_resources[WindowType.Dialog], _resources.MsgBox, UILayer.Windows, visible);
      box.Init(title, message, buttonText);
      return box;
    }

    public MessageBox CreateInfoBox(bool visible, string message)
    {
      return CreateMessageBox(visible, Tr._("Information"), message, Tr._("OK"));
    }

    public MessageBox CreateErrorBox(bool visible, string message)
    {
      return CreateMessageBox(visible, Tr._("Error"), message, Tr._("OK"));
    }
  }
}