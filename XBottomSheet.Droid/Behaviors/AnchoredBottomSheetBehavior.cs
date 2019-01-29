using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using System;
using System.Collections.Generic;
using XBottomSheet.Droid.JavaComponents;
using XBottomSheet.Droid.States;
using static Android.Support.Design.Widget.BottomSheetBehavior;

namespace XBottomSheet.Droid.Behaviors
{ 
    public sealed class AnchoredBottomSheetBehavior : CoordinatorLayout.Behavior
    {
        public const int DRAGGING_STATE = 1;
        public const int SETTLING_STATE = 2;
        public const int EXPANDED_STATE = 3;
        public const int COLLAPSED_STATE = 4;
        public const int HIDDEN_STATE = 5;
        public const int ANCHORED_STATE = 6;
        public const int PEEK_HEIGHT_AUTO = -1;

        public bool DisabledExpanded
        {
            get
            {
                return disableExpanded;
            }
            set
            {
                disableExpanded = value;
            }
        }

        public bool Hideable
        {
            get
            {
                return hideable;
            }
            set
            {
                hideable = value;
            }
        }

        public int State
        {
            get
            {
                return state;
            }
            set
            {
                if(state == value)
                {
                    return;
                }
                if(view == null)
                {
                    if (value == COLLAPSED_STATE || value == EXPANDED_STATE || value == ANCHORED_STATE ||
                        (hideable && value == HIDDEN_STATE))
                    {
                        state = value;
                    }
                    return;
                }
                var parent = (view as View).Parent;
                state = value;

                if (parent != null && parent.IsLayoutRequested && ViewCompat.IsAttachedToWindow((View)view))
                {
                    (view as View).Post(() => StartSettlingAnimation((View)view, value));
                }
                else
                {
                    StartSettlingAnimation((View)view, value);
                }
            }
        }

        public int PeekHeight
        {
            get
            {
                return peekHeightAuto ? PEEK_HEIGHT_AUTO : peekHeight;
            }
            set
            {
                bool layout = false;

                if (peekHeight == PEEK_HEIGHT_AUTO)
                {
                    if(!peekHeightAuto)
                    {
                        peekHeightAuto = true;
                        layout = true;
                    }
                }
                else if(peekHeightAuto || peekHeight != value)
                {
                    peekHeightAuto = false;
                    peekHeight = Math.Max(0, value);
                    maxOffset = parentHeight - value;
                    layout = true;
                }
                if(layout && state == COLLAPSED_STATE && view != null)
                {
                    view.RequestLayout();
                }
                peekHeight = value;
            }
        }

        public bool SkipCollapsed
        {
            get
            {
                return skipCollapsed;
            }
            set
            {
                skipCollapsed = value;
            }
        }

        public bool SkipAnchored
        {
            get
            {
                return skipAnchored;
            }
            set
            {
                skipAnchored = value;
            }
        }

        public int AnchorOffset
        {
            get
            {
                return anchorOffset;
            }
            set
            {
                if(anchorOffset != value)
                {
                    anchorOffset = value;
                }
                if(disableExpanded)
                {
                    minOffset = anchorOffset;
                }
                if(state == ANCHORED_STATE)
                {
                    SetStateInternal(SETTLING_STATE);
                    State = ANCHORED_STATE;
                }
            }
        }

        internal static readonly float HideThreshold = 0.5f;
        internal static readonly float HideFriction = 0.1f;
        internal static readonly float ExpandFriction = 0.2f;
        internal static readonly float CollapseFriction = 0.2f;

        internal DragCallback dragCallback;
        internal ViewDragHelper viewDragHelper;

        internal View view;
        internal View nestedScrollingChild;

        internal VelocityTracker velocityTracker;

        internal List<BottomSheetCallback> callbacks = new List<BottomSheetCallback>();
        internal bool ignoreEvents = false;
        internal bool skipCollapsed;
        internal bool skipAnchored;
        internal bool hideable;
        internal int peekHeight;
        internal bool nestedScrolled;
        internal bool touchingScrollingChild;
        internal int activePointerId;
        internal float minimumVelocity;
        internal float maximumVelocity;
        internal bool peekHeightAuto;
        internal bool disableExpanded;
        internal int peekHeightMin;
        internal int anchorOffset;
        internal int parentHeight;
        internal bool allowUserDragging = true;
        internal int state = COLLAPSED_STATE;

        private int initialX;
        internal int initialY;

        internal int minOffset;
        internal int maxOffset;

        public AnchoredBottomSheetBehavior() { }
        public AnchoredBottomSheetBehavior(Context context, IAttributeSet attrs) : base(context,attrs)
        {
            dragCallback = new DragCallback(this);
            TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.AnchoredBottomSheetBehavior_Layout);
            TypedValue value = a.PeekValue(Resource.Styleable.AnchoredBottomSheetBehavior_Layout_peekHeight);

            if(value != null && value.Data != PEEK_HEIGHT_AUTO)
                PeekHeight = a.GetDimensionPixelSize(Resource.Styleable.AnchoredBottomSheetBehavior_Layout_peekHeight, value.Data);
            else
                PeekHeight = a.GetDimensionPixelSize(Resource.Styleable.AnchoredBottomSheetBehavior_Layout_peekHeight, PEEK_HEIGHT_AUTO);

            Hideable = a.GetBoolean(Resource.Styleable.AnchoredBottomSheetBehavior_Layout_hideable, false);
            SkipAnchored = a.GetBoolean(Resource.Styleable.AnchoredBottomSheetBehavior_Layout_skipAnchored, false);
            SkipCollapsed = a.GetBoolean(Resource.Styleable.AnchoredBottomSheetBehavior_Layout_skipCollapsed, false);

            anchorOffset = (int)a.GetDimension(Resource.Styleable.AnchoredBottomSheetBehavior_Layout_anchorOffset, 0);
            State = a.GetInt(Resource.Styleable.AnchoredBottomSheetBehavior_Layout_defaultState, state);
            a.Recycle();

            ViewConfiguration configuration = ViewConfiguration.Get(context);
            maximumVelocity = configuration.ScaledMaximumFlingVelocity;
            minimumVelocity = configuration.ScaledMinimumFlingVelocity;
        }

        internal void Reset()
        {
            activePointerId = ViewDragHelper.InvalidPointer;
            if(velocityTracker != null)
            {
                velocityTracker.Recycle();
                velocityTracker = null;
            }
        }

        internal void SetStateInternal(int state)
        {
            if(this.state == state)
            {
                return;
            }
            var oldState = this.state;
            this.state = state;
            var bottomSheet = view;

            if(bottomSheet != null)
            {
                for(int i=0;i<callbacks.Count;i++)
                {
                    callbacks[i].OnStateChanged(bottomSheet, state);
                }
            }
        }

        internal void StartSettlingAnimation(View child,int state)
        {
            int top = 0;

            switch(state)
            {
                case COLLAPSED_STATE:
                    top = maxOffset;
                    break;

                case EXPANDED_STATE:
                    top = minOffset;
                    break;

                case ANCHORED_STATE:
                    if (anchorOffset > minOffset)
                    {
                        top = anchorOffset;
                    }
                    else
                    {
                        state = EXPANDED_STATE;
                        top = minOffset;
                    }
                    break;
                case HIDDEN_STATE:
                    top = parentHeight;
                    break;
            }
            SetStateInternal(SETTLING_STATE);

            if(viewDragHelper.SmoothSlideViewTo(child,child.Left,top))
            {
                ViewCompat.PostOnAnimation(child, new SettleRunnable((View)child, state, viewDragHelper, SetStateInternal));
            }
        }

        internal void DispatchOnSlide(int top)
        {
            var bottomSheet = view;
            if(bottomSheet != null)
            {
                float slideOffset;
                if(top > maxOffset)
                {
                    slideOffset = (float)(maxOffset - top) / (parentHeight - maxOffset);
                }
                else
                {
                    slideOffset = (float)(maxOffset - top) / (maxOffset - minOffset);
                }

                for(int i = 0; i < callbacks.Count;i++)
                {
                    callbacks[i].OnSlide(bottomSheet, slideOffset);
                }
            }
        }

        internal bool ShouldExpand(View child, float yVelocity)
        {
            if(skipAnchored || minOffset >= anchorOffset)
            {
                return true;
            }
            int currentTop = child.Top;
            if(currentTop < anchorOffset)
            {
                return true;
            }
            float newTop = currentTop + yVelocity * ExpandFriction;
            return newTop < anchorOffset;
        }

        internal bool ShouldCollapse(View child, float yVelocity)
        {
            if(skipAnchored || minOffset >= anchorOffset)
            {
                return true;
            }
            int currentTop = child.Top;
            if(currentTop > anchorOffset)
            {
                return true;
            }
            float newTop = currentTop + yVelocity * CollapseFriction;
            return newTop > anchorOffset;
        }

        internal bool ShouldHide(View child,float yVelocity)
        {
            if(skipCollapsed)
            {
                return true;
            }
            if(child.Top < maxOffset)
            {
                return false;
            }
            float newTop = child.Top + yVelocity * HideFriction;
            return Math.Abs(newTop - maxOffset) / (float)peekHeight > HideThreshold;
        }

        internal void CalculateTopAndTargetState(View child, float xVelocity, float yVelocity, int[] output)
        {
            int top = 0;
            int targetState = 0;

            if(yVelocity < 0 && Math.Abs(yVelocity) > minimumVelocity && Math.Abs(yVelocity) > Math.Abs(xVelocity))
            {
                if(ShouldExpand(child,yVelocity))
                {
                    top = minOffset;
                    targetState = EXPANDED_STATE;
                }
                else
                {
                    top = anchorOffset;
                    targetState = ANCHORED_STATE;
                }
            }
            else if(hideable && ShouldHide(child,yVelocity))
            {
                top = parentHeight;
                targetState = HIDDEN_STATE;
            }
            else if(yVelocity > 0 && Math.Abs(yVelocity) > minimumVelocity && Math.Abs(yVelocity) > Math.Abs(xVelocity))
            {
                if(ShouldCollapse(child,yVelocity))
                {
                    top = maxOffset;
                    targetState = COLLAPSED_STATE;
                }
                else
                {
                    top = AnchorOffset;
                    targetState = ANCHORED_STATE;
                }
            }
            else
            {
                int currentTop = child.Top;
                int distanceToExpanded = Math.Abs(currentTop - minOffset);
                int distanceToCollapsed = Math.Abs(currentTop - maxOffset);
                int distanceToAnchor = Math.Abs(currentTop - anchorOffset);

                if(anchorOffset > minOffset && distanceToAnchor < distanceToExpanded && distanceToAnchor < distanceToCollapsed)
                {
                    top = anchorOffset;
                    targetState = ANCHORED_STATE;
                }
                else if(distanceToExpanded < distanceToCollapsed)
                {
                    top = minOffset;
                    targetState = EXPANDED_STATE;
                }
                else
                {
                    top = maxOffset;
                    targetState = COLLAPSED_STATE;
                }
            }

            output[0] = top;
            output[1] = targetState;
        }
        internal View FindScrollingChild(View view)
        {
            if (view is INestedScrollingChild)
                return view;
            return null;
        }

        public override IParcelable OnSaveInstanceState(CoordinatorLayout parent, Java.Lang.Object child)
        {
            return new SavedState(base.OnSaveInstanceState(parent, child), state);
        }
        public override void OnRestoreInstanceState(CoordinatorLayout parent, Java.Lang.Object child, IParcelable state)
        {
            SavedState ss = (SavedState)state;

            base.OnRestoreInstanceState(parent, child, ss.SuperState);

            if(ss.state == DRAGGING_STATE || ss.state == SETTLING_STATE)
            {
                this.state = COLLAPSED_STATE;
            }
            else
            {
                this.state = ss.state;
            }
        }

        public override bool OnLayoutChild(CoordinatorLayout parent, Java.Lang.Object child, int layoutDirection)
        {
            if (ViewCompat.GetFitsSystemWindows(parent) && !ViewCompat.GetFitsSystemWindows((View)child))
            {
                ViewCompat.SetFitsSystemWindows((View)child, true);
            }

            var savedTop = (child as View).Top;

            parent.OnLayoutChild((View)child, layoutDirection);

            parentHeight = parent.Height;

            int peekHeight;

            if(peekHeightAuto)
            {
                if(peekHeightMin == 0)
                {
                    peekHeightMin = 10;
                }
                peekHeight = Math.Max(peekHeightMin, parentHeight - parent.Width * 9 / 16);
            }
            else
            {
                peekHeight = this.peekHeight;
            }
            minOffset = Math.Max(0, parentHeight - (child as View).Height);

            if(disableExpanded)
            {
                minOffset = anchorOffset;
            }

            maxOffset = Math.Max(parent.Height - peekHeight, minOffset);

            switch(State)
            {
                case EXPANDED_STATE:
                    ViewCompat.OffsetTopAndBottom((View)child, minOffset);
                    break;

                case HIDDEN_STATE:
                    if(hideable)
                        ViewCompat.OffsetTopAndBottom((View)child, parentHeight);
                    break;

                case COLLAPSED_STATE:
                    ViewCompat.OffsetTopAndBottom((View)child, maxOffset);
                    break;

                case DRAGGING_STATE:
                    ViewCompat.OffsetTopAndBottom((View)child, savedTop - (child as View).Top);
                    break;

                case SETTLING_STATE:
                    ViewCompat.OffsetTopAndBottom((View)child, savedTop - (child as View).Top);
                    break;

                case ANCHORED_STATE:
                    if (anchorOffset > minOffset)
                    {
                        ViewCompat.OffsetTopAndBottom((View)child, anchorOffset);
                    }
                    else
                    {
                        State = EXPANDED_STATE;
                        ViewCompat.OffsetTopAndBottom((View)child, minOffset);
                    }
                    break;
            }

            if(viewDragHelper == null && dragCallback != null)
            {
                viewDragHelper = ViewDragHelper.Create(parent, dragCallback);
            }

            view = child as View;
            nestedScrollingChild = FindScrollingChild((View)child);

            return true;
        }

        public override bool OnInterceptTouchEvent(CoordinatorLayout parent, Java.Lang.Object child, MotionEvent ev)
        {
            if((child as View).IsShown || !allowUserDragging)
            {
                ignoreEvents = true;
                return false;
            }

            var action = MotionEventCompat.GetActionMasked(ev);

            if (action == (int)MotionEventActions.Down)
            {
                Reset();
            }

            if(velocityTracker == null)
            {
                velocityTracker = VelocityTracker.Obtain();
            }

            velocityTracker.AddMovement(ev);

            switch(action)
            {
                case (int)MotionEventActions.Up:
                case (int)MotionEventActions.Cancel:
                    touchingScrollingChild = false;
                    activePointerId = MotionEvent.InvalidPointerId;

                    if(ignoreEvents)
                    {
                        ignoreEvents = false;
                        return false;
                    }
                    break;
                case (int)MotionEventActions.Down:
                    initialX = (int)ev.GetX();
                    initialY = (int)ev.GetY();
                    var scroll = nestedScrollingChild;

                    if (scroll != null && parent.IsPointInChildBounds(scroll,initialX,initialY))
                    {
                        activePointerId = ev.GetPointerId(ev.ActionIndex);
                        touchingScrollingChild = true;
                    }
                    ignoreEvents = activePointerId == MotionEvent.InvalidPointerId && !parent.IsPointInChildBounds((View)child, initialX, initialY);
                    break;
            }
            if(!ignoreEvents && viewDragHelper.ShouldInterceptTouchEvent(ev))
            {
                return true;
            }

            var scrollView = nestedScrollingChild;

            return action == (int)MotionEventActions.Move && scrollView != null &&
                    !ignoreEvents && state != DRAGGING_STATE &&
                    !parent.IsPointInChildBounds(scrollView, (int)ev.GetX(), (int)ev.GetY()) &&
                     Math.Abs(initialY - ev.GetY()) > viewDragHelper.TouchSlop;
        }

        public override bool OnTouchEvent(CoordinatorLayout parent, Java.Lang.Object child, MotionEvent ev)
        {
            if(!(child as View).IsShown || !allowUserDragging)
            {
                return false;
            }

            var action = MotionEventCompat.GetActionMasked(ev);

            if (state == DRAGGING_STATE && action == (int)MotionEventActions.Down)
            {
                return true;
            }

            if (viewDragHelper != null)
            {
                viewDragHelper.ProcessTouchEvent(ev);
            }

            if(action == (int)MotionEventActions.Down)
            {
                Reset();
            }

            if(velocityTracker == null)
            {
                velocityTracker = VelocityTracker.Obtain();
            }
            velocityTracker.AddMovement(ev);

            if(action == (int)MotionEventActions.Move && !ignoreEvents)
            {
                if(Math.Abs(initialY - ev.GetY()) > viewDragHelper.TouchSlop)
                {
                    viewDragHelper.CaptureChildView((View)child, ev.GetPointerId(ev.ActionIndex));
                }
            }

            return !ignoreEvents;
        }

        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int axes)
        {
            if(!allowUserDragging)
            {
                return false;
            }
            nestedScrolled = false;

            return (axes & ViewCompat.ScrollAxisVertical) != 0;
        }

        public override void OnNestedPreScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dx, int dy, int[] consumed)
        {
            if(!allowUserDragging)
            {
                return;
            }

            var scrollingChild = nestedScrollingChild;

            if(target != scrollingChild)
            {
                return;
            }

            var currentTop = (child as View).Top;
            int newTop = currentTop - dy;

            if (dy > 0)
            {
                if(newTop < minOffset)
                {
                    consumed[1] = currentTop - minOffset;
                    ViewCompat.OffsetTopAndBottom((View)child, -consumed[1]);
                    SetStateInternal(EXPANDED_STATE);
                }
                else
                {
                    consumed[1] = dy;
                    ViewCompat.OffsetTopAndBottom((View)child, -dy);
                    SetStateInternal(DRAGGING_STATE);
                }
            }
            else if(dy < 0)
            {
                if(!ViewCompat.CanScrollVertically(target,-1))
                {
                    if(newTop <= maxOffset || Hideable)
                    {
                        consumed[1] = dy;
                        ViewCompat.OffsetTopAndBottom((View)child, -dy);
                        SetStateInternal(DRAGGING_STATE);
                    }
                    else
                    {
                        consumed[1] = currentTop - maxOffset;
                        ViewCompat.OffsetTopAndBottom((View)child, -consumed[1]);
                        SetStateInternal(COLLAPSED_STATE);
                    }
                }
            }
            DispatchOnSlide((child as View).Top);
            nestedScrolled = true;
        }

        public override void OnStopNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target)
        {
            if(!allowUserDragging)
            {
                return;
            }
            if((child as View).Top == minOffset)
            {
                SetStateInternal(EXPANDED_STATE);
                return;
            }
            if(target != nestedScrollingChild || !nestedScrolled)
            {
                return;
            }

            velocityTracker.ComputeCurrentVelocity(1000, maximumVelocity);

            var xVelocity = velocityTracker.GetXVelocity(activePointerId);
            var yVelocity = velocityTracker.GetYVelocity(activePointerId);

            int[] output = new int[2];
            CalculateTopAndTargetState((View)child, xVelocity, yVelocity, output);

            int top = output[0];
            int targetState = output[1];

            if (viewDragHelper.SmoothSlideViewTo((View)child, (child as View).Left, top))
            {
                SetStateInternal(SETTLING_STATE);
                ViewCompat.PostOnAnimation((View)child, new SettleRunnable((View)child, targetState, viewDragHelper, SetStateInternal));
            }
            else
            {
                SetStateInternal(targetState);
            }
            nestedScrolled = false;
        }

        public override bool OnNestedPreFling(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, float velocityX, float velocityY)
        {
            if(!allowUserDragging)
            {
                return false;
            }
            return target == nestedScrollingChild && (state != EXPANDED_STATE || base.OnNestedPreFling(coordinatorLayout, child, target, velocityX, velocityY));
        }

        internal sealed class DragCallback : ViewDragHelper.Callback
        {
            internal AnchoredBottomSheetBehavior parentClass;

            internal DragCallback(AnchoredBottomSheetBehavior parentClass)
            {
                this.parentClass = parentClass;
            }

            public override bool TryCaptureView(View child, int pointerId)
            {
                if (parentClass.state == DRAGGING_STATE)
                {
                    return false;
                }
                if(parentClass.touchingScrollingChild)
                {
                    return false;
                }
                if(parentClass.state == EXPANDED_STATE && parentClass.activePointerId == pointerId)
                {
                    var scroll = parentClass.nestedScrollingChild;
                    if(scroll != null && ViewCompat.CanScrollVertically(scroll,-1))
                    {
                        return false;
                    }
                }
                return parentClass.view != null && parentClass.view == child;
            }
            public override void OnViewPositionChanged(View changedView, int left, int top, int dx, int dy)
            {
                parentClass.DispatchOnSlide(top);
            }

            public override void OnViewDragStateChanged(int state)
            {
                if(state == ViewDragHelper.StateDragging)
                {
                    parentClass.SetStateInternal(DRAGGING_STATE);
                }
            }

            public override void OnViewReleased(View releasedChild, float xvel, float yvel)
            {
                int[] output = new int[2];
                parentClass.CalculateTopAndTargetState(releasedChild, xvel, yvel, output);

                int top = output[0];
                int targetState = output[1];

                if(parentClass.viewDragHelper.SettleCapturedViewAt(releasedChild.Left,top))
                {
                    parentClass.SetStateInternal(SETTLING_STATE);
                    ViewCompat.PostOnAnimation(releasedChild, new SettleRunnable(releasedChild, targetState, parentClass.viewDragHelper, parentClass.SetStateInternal));
                }
                else
                {
                    parentClass.SetStateInternal(targetState);
                }
            }

            public override int ClampViewPositionVertical(View child, int top, int dy)
            {
                return Constrain(top, parentClass.minOffset, parentClass.hideable ? parentClass.parentHeight : parentClass.maxOffset);
            }

            public override int ClampViewPositionHorizontal(View child, int left, int dx)
            {
                return child.Left;
            }

            public override int GetViewVerticalDragRange(View child)
            {
                if(parentClass.hideable)
                {
                    return parentClass.parentHeight - parentClass.minOffset;
                }
                else
                {
                    return parentClass.maxOffset - parentClass.minOffset;
                }
            }

            private int Constrain(int amount, int low, int high)
            {
                return amount < low ? low : (amount > high ? high : amount);
            }
        }
    }
}