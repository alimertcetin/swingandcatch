using System;
using UnityEngine;

namespace TheGame.UISystems.TabSystem
{
    public static class TabHelper
    {
        public static void GetDirections(TabMovementDirection tabMovementDirection, Vector2 targetPos, Vector2 currentPos, out Direction targetEntirDirection, out Direction currentExitDirection)
        {
            switch (tabMovementDirection)
            {
                case TabMovementDirection.Horizontal:
                    var targetPageX = targetPos.x;
                    var currentPageX = currentPos.x;
                    if (Mathf.Abs(targetPageX - currentPageX) - Mathf.Epsilon < Mathf.Epsilon)
                    {
                        targetPageX = targetPos.y;
                        currentPageX = currentPos.y;
                    }
                    bool check1 = targetPageX > currentPageX;
                    targetEntirDirection = check1 ? Direction.Right : Direction.Left;
                    currentExitDirection = check1 ? Direction.Left : Direction.Right;
                    break;
                case TabMovementDirection.Vertical:
                    var targetPageY = targetPos.y;
                    var currentPageY = currentPos.y;
                    if (Mathf.Abs(targetPageY - currentPageY) - Mathf.Epsilon < Mathf.Epsilon)
                    {
                        targetPageY = targetPos.x;
                        currentPageY = currentPos.x;
                    }

                    bool check2 = targetPageY > currentPageY;
                    targetEntirDirection = check2 ? Direction.Up : Direction.Down;
                    currentExitDirection = check2 ? Direction.Down : Direction.Up;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}