using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using FirstAlert.Fragments;

namespace FirstAlert
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_camera);
            if (savedInstanceState == null)
                FragmentManager.BeginTransaction()
                    .Replace(Resource.Id.container, CameraFragment.NewInstance())
                    .Commit();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}