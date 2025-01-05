using UnityEngine;

namespace NoaDebugger
{
    sealed class ContentSizeFitterByChildren : MonoBehaviour
    {
        RectTransform _rect;
        
        void Start()
        {
            _rect = transform as RectTransform;
            _WidthFitterByChildren();
        }
    
        void _WidthFitterByChildren()
        {
            var corners = new Vector3[4];
            _rect.GetWorldCorners(corners);
            var leftWorldCorner = corners[1]; 
            var rightWorldCorner = corners[2]; 
    
            var rightmostWorldPosition = _GetRightmostWorldPosition(_rect, rightWorldCorner);
    
            var leftLocalCorner = transform.InverseTransformPoint(leftWorldCorner);
            var rightmostLocalCorner = transform.InverseTransformPoint(rightmostWorldPosition);
    
            var width = rightmostLocalCorner.x - leftLocalCorner.x;
            
            _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }
    
        Vector3 _GetRightmostWorldPosition(RectTransform rect, Vector3 rightmostWorldPosition)
        {
            foreach (RectTransform child in rect)
            {
                rightmostWorldPosition = _GetRightmostWorldPosition(child, rightmostWorldPosition);
            }
    
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            var rightWorldCorner = corners[2]; 
            if (rightWorldCorner.x > rightmostWorldPosition.x)
            {
                rightmostWorldPosition.x = rightWorldCorner.x;
            }
    
            return rightmostWorldPosition;
        }
    }
}
