using System.Collections.Generic;
using DPong.UI.Holder;
using UnityEngine;

namespace DPong.UI {
  public class UISystem {
    private readonly Transform _root;
    private readonly Dictionary<UILayer, Transform> _layers = new Dictionary<UILayer, Transform>();

    private readonly UISystemResources _resources;

    public UISystem(Canvas canvas) {
      _root = canvas.transform;
      _resources = Resources.Load<UISystemResources>("UISystemResources");

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
      var ui = Object.Instantiate(prefab, _layers[layer]);
      ui.SetInitialVisibility(visible);
      return ui;
    }

    public T InstantiateWrapped<T>(UIHolder holder, UILayer layer, bool visible) where T : UIHolderWrapper {
      var holderInstance = Object.Instantiate(holder, _layers[layer]);
      var wrapperInstance = holderInstance.ContentRoot.GetComponent<T>();

      wrapperInstance.WrapHolder(holderInstance);
      wrapperInstance.SetInitialVisibility(visible);

      return wrapperInstance;
    }

    public T InstantiateAndWrap<T>(UIHolder holder, T wrapper, UILayer layer, bool visible) where T : UIHolderWrapper {
      var holderInstance = Object.Instantiate(holder, _layers[layer]);
      var wrapperInstance = Object.Instantiate(wrapper, holderInstance.ContentRoot);

      wrapperInstance.WrapHolder(holderInstance);
      wrapperInstance.SetInitialVisibility(visible);

      return wrapperInstance;
    }

    public T InstantiateWindow<T>(WindowType winType, T wrapper, bool visible) where T : UIHolderWrapper {
      return InstantiateAndWrap(_resources[winType], wrapper, UILayer.Windows, visible);
    }
  }
}