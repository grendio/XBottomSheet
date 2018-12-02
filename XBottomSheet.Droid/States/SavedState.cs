using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Java.Lang;

namespace XBottomSheet.Droid.States
{
    public class SavedState : AbsSavedState
    {
        public int state;

        public SavedState(Parcel source) : base(source,null)
        {
        }

        public SavedState(Parcel source, ClassLoader loader) : base(source, loader)
        {
            state = source.ReadInt();
        }

        public SavedState(Parcel baseState, int state) : base(baseState)
        {
            this.state = state;
        }

        public SavedState(IParcelable baseState, int state) : base(baseState)
        {
            this.state = state;
        }

        public override void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            base.WriteToParcel(dest, flags);
            dest.WriteInt(state);
        }
    }
}