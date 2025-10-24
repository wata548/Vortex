using System;
using UnityEngine;

namespace Extension {

    public struct Pivot {

        public static readonly Pivot Up = 
            new(PivotLocation.Up, PivotLocation.Up);
        public static readonly Pivot Middle = 
            new(PivotLocation.Middle, PivotLocation.Middle);
        public static readonly Pivot Down = 
            new(PivotLocation.Down, PivotLocation.Down);
        
        public readonly PivotLocation X;
        public readonly PivotLocation Y;

        public Pivot(PivotLocation x = PivotLocation.Middle, PivotLocation y = PivotLocation.Middle) => 
            (X, Y) = (x, y);
    } 
    
    //Up
    //Middle
    //Down Middle Up
    public enum PivotLocation {
        Middle,
        Up,
        Down,
    };
    [Flags]
    public enum Axes {
        None = 0b00,
        X = 0b10,
        Y = 0b01,
    }
    
    public static class ExRect {

       //==================================================||ScreenSize 
        public static void GetScreenSize(this RectTransform canvas, out Vector2 result, int x = 1, int y = 1) =>
            result = new(canvas.rect.width * x, canvas.rect.height * y);
        
        public static float GetScreenSizeX(this RectTransform canvas, float ratio = 1) => 
            canvas.rect.width * ratio;

        public static float GetScreenSizeY(this RectTransform canvas, float ratio = 1) =>
            canvas.rect.height * ratio;
        
       //==================================================||scale 
        public static void SetScale(this RectTransform rect, RectTransform canvas, Vector2 ratio) =>
            rect.sizeDelta = new(canvas.rect.width * ratio.x, canvas.rect.height * ratio.y);
        public static void SetScale(this RectTransform rect, Vector2 delta) =>
            rect.sizeDelta = delta;
        public static void AddScale(this RectTransform rect, Vector2 delta) {
            var result = rect.sizeDelta;
            result.x += delta.x;
            result.y += delta.y;
            
            rect.sizeDelta = result;
        }
        public static void MultipleScale(this RectTransform rect, float ratio, Axes axes) {

            var result = rect.sizeDelta;
            if ((axes & Axes.X) == Axes.X)
                result.x *= ratio;
            if ((axes & Axes.Y) == Axes.Y)
                result.y *= ratio;

            rect.sizeDelta = result;
        }

        public static void MultipleScale(this RectTransform rect, Vector2 delta) =>
            rect.sizeDelta = rect.sizeDelta * delta;

        public static Vector2 GetParentSize(this RectTransform rect) {
            return (rect.parent as RectTransform)!.sizeDelta;
        } 
        
        public static void SetLocalScale(this RectTransform rect, Vector2 ratio) =>
            rect.sizeDelta = ratio * rect
                .parent
                .GetComponent<RectTransform>()
                .sizeDelta;

        public static void SetLocalScaleX(this RectTransform rect, float value) {
            var result = new Vector2(value, 0) * rect
                .parent
                .GetComponent<RectTransform>()
                .sizeDelta;

            result.y = rect.sizeDelta.y;
            rect.sizeDelta = result;
        }
        
        public static void SetLocalScaleY(this RectTransform rect, float value) {
            var result = new Vector2(0, value) * rect
                .parent
                .GetComponent<RectTransform>()
                .sizeDelta;
        
            result.x = rect.sizeDelta.x;
            rect.sizeDelta = result;
        }

        public static Vector2 GetSizeAmount(this RectTransform rect, Vector2 value) =>
            rect.sizeDelta * value;

        public static float GetSizeAmountX(this RectTransform rect, float x = 1) =>
            rect.sizeDelta.x * x;
        
        public static float GetSizeAmountY(this RectTransform rect, float y = 1) =>
                    rect.sizeDelta.y * y;
        
       //==================================================||Position

       public static void SetPositionX(this RectTransform rect, float x) =>
           rect.position = new(x, rect.position.y, rect.position.z);
       public static void SetPositionY(this RectTransform rect, float y) =>
           rect.position = new(rect.position.x, y, rect.position.z); 
       public static void SetPositionZ(this RectTransform rect, float z) =>
           rect.position = new(rect.position.x, rect.position.y, z);
       
       public static void AddPosition(this RectTransform rect, Vector2 position) =>
           rect.position = rect.position.ToVec2() + position;

       public static void AddPositionX(this RectTransform rect, float x) =>
           rect.AddPosition(new(x, 0));
       
       public static void AddPositionY(this RectTransform rect, float y) =>
                  rect.AddPosition(new(0, y));

       public static void SetPosition(this RectTransform rect, Vector2 postion) =>
           rect.position = postion;
       
       public static void SetPosition(this RectTransform rect, RectTransform canvas, Pivot pivot) =>
           SetPosition(rect, canvas, pivot, Vector2.zero);
       public static void SetPosition(this RectTransform rect, RectTransform canvas, Vector2 ratio) =>
           SetPosition(rect, canvas, new(PivotLocation.Middle, PivotLocation.Middle), ratio);
       public static void SetPosition(this RectTransform rect, RectTransform canvas, Pivot pivot, Vector2 ratio) {
           var result = Vector2.zero;
           
           result.x = pivot.X switch {
               PivotLocation.Down => ratio.x * canvas.rect.width,
               PivotLocation.Middle => (0.5f + ratio.x) * canvas.rect.width,
               PivotLocation.Up => (1 + ratio.x) * canvas.rect.width,
               _ => 0,
           };
           result.y = pivot.Y switch {
               PivotLocation.Down => ratio.y * canvas.rect.height,
               PivotLocation.Middle => (0.5f + ratio.y) * canvas.rect.height,
               PivotLocation.Up => (1 + ratio.y) * canvas.rect.height,
               _ => 0,
           };

           rect.position = result;
       }

       /// <summary>
       /// This function assumes Virtual pivot is located at the center(0,0).
       /// and move the object so that the virtual pivot aligns with the given pivot.
       /// (virtual pivot's position never changed)
       /// </summary>
       public static void ChangeVirtualPivot(this RectTransform rect, Pivot pivot) {
           var result = Vector2.zero;
           var scale = rect.sizeDelta;

           result.x = pivot.X switch {
               PivotLocation.Up => -scale.x / 2,
               PivotLocation.Down => scale.x / 2,
               _ => 0,
           };
           
           result.y = pivot.Y switch {
               PivotLocation.Up => -scale.y / 2,
               PivotLocation.Down => scale.y / 2,
               _ => 0,
           };

           rect.position = rect.position.ToVec2() + result;
       }

       public static Vector2 GetLocalPositionY(this RectTransform rect, PivotLocation pivot = PivotLocation.Middle, float position = 0f) {
                  
           var parentScale = rect.parent!.GetComponent<RectTransform>().sizeDelta;
           var result = rect.localPosition;
                  
           result.y = pivot switch {
               PivotLocation.Down => (position - 0.5f) * parentScale.y,
               PivotLocation.Middle => position * parentScale.y,
               PivotLocation.Up => (position + 0.5f) * parentScale.y,
               _ => 0,
           };
       
           return result;
       }
       
       public static Vector2 GetLocalPositionX(this RectTransform rect, PivotLocation pivot = PivotLocation.Middle, float position = 0f) {
                  
           var parentScale = rect.parent!.GetComponent<RectTransform>().sizeDelta;
           var result = rect.localPosition;
                  
           result.x = pivot switch {
               PivotLocation.Down => (position - 0.5f) * parentScale.x,
               PivotLocation.Middle => position * parentScale.x,
               PivotLocation.Up => (position + 0.5f) * parentScale.x,
               _ => 0,
           };
       
           return result;
       }
       
       public static void SetLocalPositionX(this RectTransform rect, PivotLocation pivot, float position = 0f) {
           
           var parentScale = rect.parent!.GetComponent<RectTransform>().sizeDelta;
           var result = rect.localPosition;
           
           result.x = pivot switch {
               PivotLocation.Down => (position - 0.5f) * parentScale.x,
               PivotLocation.Middle => position * parentScale.x,
               PivotLocation.Up => (position + 0.5f) * parentScale.x,
               _ => 0,
           };

           rect.localPosition = result;
       }

       public static void SetLocalPositionY(this RectTransform rect, PivotLocation pivot = PivotLocation.Middle, float position = 0f) {
                  
           var parentScale = rect.parent!.GetComponent<RectTransform>().sizeDelta;
           var result = rect.localPosition;
                  
           result.y = pivot switch {
               PivotLocation.Down => (position - 0.5f) * parentScale.y,
               PivotLocation.Middle => position * parentScale.y,
               PivotLocation.Up => (position + 0.5f) * parentScale.y,
               _ => 0,
           };
       
           rect.localPosition = result;
       }
       
       public static void SetLocalPosition(this RectTransform rect, Pivot pivot) =>
           rect.SetLocalPosition(pivot, Vector2.zero);
       
       public static void SetLocalPosition(this RectTransform rect, Vector2 ratio) =>
            rect.SetLocalPosition(Pivot.Middle, ratio);

       public static Vector2 GetLocalPosition(this RectTransform rect, Pivot pivot) {
           var parentScale = rect.parent!.GetComponent<RectTransform>().sizeDelta;
           var result = rect.localPosition / parentScale;
           
           result.x += pivot.X switch {
           
               PivotLocation.Down => 0.5f,
               PivotLocation.Up => -0.5f,
               _ => 0,
           };
           
           result.y += pivot.Y switch {
           
               PivotLocation.Down => 0.5f,
               PivotLocation.Up => -0.5f,
               _ => 0,
           };

           return result;
       }
       
       public static void SetLocalPosition(this RectTransform rect, Pivot pivot, Vector2 ratio) {
           var parentScale = rect.parent!.GetComponent<RectTransform>().sizeDelta;
           var result = Vector2.zero;

           result.x = pivot.X switch {

               PivotLocation.Down => (ratio.x - 0.5f) * parentScale.x,
               PivotLocation.Middle => ratio.x * parentScale.x,
               PivotLocation.Up => (ratio.x + 0.5f) * parentScale.x,
               _ => 0,
           };

           result.y = pivot.Y switch {

               PivotLocation.Down => (ratio.y - 0.5f) * parentScale.y,
               PivotLocation.Middle => ratio.y * parentScale.y,
               PivotLocation.Up => (ratio.y + 0.5f) * parentScale.y,
               _ => 0,
           };
           
           rect.localPosition = result;
       }
    }
}