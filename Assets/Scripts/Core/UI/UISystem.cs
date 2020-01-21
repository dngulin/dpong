using System.Collections.Generic;
using DPong.Core.UI.Holder;
using UnityEngine;

namespace DPong.Core.UI {
  public class UISystem {
    private readonly Transform _root;
    private readonly Dictionary<UILayer, Transform> _layers = new Dictionary<UILayer, Transform>();

    private readonly UISystemResources _resources;

    public UISystem(Canvas canvas) {
      _root = canvas.transform;
      _resources = Resources.Load<UISystemResources>("UISystemResources");

      foreach (var layer in new[] {UILayer.Background, UILayer.Panels, UILayer.Windows, UILayer.Foreground})
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

    public THolder InstantiateHolder<THolder>(THolder holder, UILayer layer, bool visible) where THolder : UIHolder {
      var holderInstance = Object.Instantiate(holder, _layers[layer]);
      holderInstance.InternalInit(visible);

      return holderInstance;
    }

    public T InstantiateInLayer<T>(T prefab, UILayer layer) where T : MonoBehaviour {
      return Object.Instantiate(prefab, _layers[layer]);
    }

    public TContent InstantiateHolder<THolder, TContent>(THolder holder, TContent content, UILayer layer, bool visible)
      where THolder : UIHolder where TContent : UIHolderContent {
      var holderInstance = Object.Instantiate(holder, _layers[layer]);
      holderInstance.InternalInit(visible);

      var contentInstance = Object.Instantiate(content, holderInstance.ContentRoot);
      contentInstance.InternalInit(holderInstance);

      return contentInstance;
    }

    public TContent InstantiateWindow<TContent>(WindowType type, TContent content, bool visible)
      where TContent : UIHolderContent {
      return InstantiateHolder(_resources.Windows[type], content, UILayer.Windows, visible);
    }

    public TContent InstantiatePanel<TContent>(PanelType type, TContent content, bool visible)
      where TContent : UIHolderContent {
      return InstantiateHolder(_resources.Panels[type], content, UILayer.Panels, visible);
    }
  }
}