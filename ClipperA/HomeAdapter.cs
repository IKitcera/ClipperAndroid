using AbdulAris.Civ;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.ViewPager.Widget;
using Bumptech.Glide;
using Clipper.Models;
using Clipper.ViewModels;
using System;
using System.Collections.Generic;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace ClipperA
{
    class HomeAdapter : BaseAdapter<PhotoPost>
    {
        List<PhotoPost> posts;
        List<string> avtrs;
        List<string> nicks;
        Fragment context;

        public event Action<string> ItemClick;
        Action<string> listener;
        public HomeAdapter(Fragment context, HomeViewModel home) : base()
        {
            this.context = context;

            posts = home.Flow;
            avtrs = home.UsersAvatars;
            nicks = home.UsersNames;

            listener = OnClick;
        }
        public override PhotoPost this[int position] => posts[position];

        public override int Count => posts.Count;

        public override long GetItemId(int position) => position;


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var post = posts[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.Post, null);

            var avtr = view.FindViewById<CircleImageView>(Resource.Id.userIcon);

            avtr.Click += (sender, e) =>
            {
                listener(post.UserId);
            };

            if (avtrs[position] != "")
            {
                Glide.With(context).Load(avtrs[position]).Into(avtr);
            }
            else
            {
                Glide.With(context).Load("https://st3.depositphotos.com/1156795/35622/v/600/depositphotos_356226476-stock-illustration-profile-placeholder-image-gray-silhouette.jpg").Into(avtr);
            }

            ViewPager viewPager = view.FindViewById<ViewPager>(Resource.Id.vwPager);
            if (viewPager == null) // no view to re-use, create new
                view = context.Activity.LayoutInflater.Inflate(Resource.Layout.PhotoItem, null);

            PhotoAdapter adapter = new PhotoAdapter(context.Activity, posts[position].Images);
            viewPager.Adapter = adapter;

            view.FindViewById<TextView>(Resource.Id.cmnt).Text = post.Comments.Count.ToString();
            view.FindViewById<TextView>(Resource.Id.reactionUp).Text = post.Reactions.FindAll(r => r == Reaction.Positive).Count.ToString();
            view.FindViewById<TextView>(Resource.Id.reactionDown).Text = post.Reactions.FindAll(r => r == Reaction.Negative).Count.ToString();
            view.FindViewById<TextView>(Resource.Id.txtBelow).Text = post.TextBelow;
            view.FindViewById<TextView>(Resource.Id.userName).Text = nicks[position];
            return view;
        }

        void OnClick(string uId)
        {
            if (ItemClick != null)
                ItemClick(uId);
        }
    }

    class PhotoAdapter : PagerAdapter
    {
        Context context;
        public List<string> photos;
        public override int Count => photos.Count;

        public PhotoAdapter(Context context, List<string> photos)
        {
            this.context = context;
            this.photos = photos;
        }

        // Create the tree page for the given position:
        public override Java.Lang.Object InstantiateItem(View container, int position)
        {
            var imageView = new ImageView(context);

            Glide.With(context).Load(photos[position]).Into(imageView);

            var viewPager = container.JavaCast<ViewPager>();
            viewPager.AddView(imageView);
            return imageView;
        }

        // Remove a tree page from the given position.
        public override void DestroyItem(View container, int position, Java.Lang.Object view)
        {
            var viewPager = container.JavaCast<ViewPager>();
            viewPager.RemoveView(view as View);
        }

        // Determine whether a page View is associated with the specific key object
        // returned from InstantiateItem (in this case, they are one in the same):
        public override bool IsViewFromObject(View view, Java.Lang.Object obj)
        {
            return view == obj;
        }

        // Display a caption for each Tree page in the PagerTitleStrip:
        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            if (Count == 1)
                return new Java.Lang.String("");
            return new Java.Lang.String((position + 1).ToString() + "/" + Count.ToString());
        }
    }
}