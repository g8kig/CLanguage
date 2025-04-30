﻿using System;
using CoreGraphics;

#if __IOS__
using UIKit;
using NativeColor = UIKit.UIColor;
using NativeFont = UIKit.UIFont;
using NativeGraphics = UIKit.UIGraphics;
using NativeStringAttributes = UIKit.UIStringAttributes;
using NativeBezierPath = UIKit.UIBezierPath;
#elif __MACOS__
using AppKit;
using NativeColor = AppKit.NSColor;
using NativeFont = AppKit.NSFont;
using NativeGraphics = AppKit.NSGraphics;
using NativeStringAttributes = AppKit.NSStringAttributes;
using NativeBezierPath = AppKit.NSBezierPath;
#endif

using static CLanguage.Editor.Extensions;

using Foundation;

namespace CLanguage.Editor
{
    class ErrorView : DrawingView
    {
        Report.AbstractMessage message = new Report.AbstractMessage ("Info", "");
        public Report.AbstractMessage Message {
            get => message;
            set {
                message = value;
                SetNeedsDisplayInRect (Bounds);
                InvalidateIntrinsicContentSize ();
                AlphaValue = string.IsNullOrEmpty (message.Text) ? 0 : 1;
            }
        }

        public ErrorView ()
        {
            BackgroundColor = NativeColor.Clear;
        }

        const float hpad = 18;

        public override CGSize IntrinsicContentSize {
            get {
                var mt = MessageText;

                var amt = new NSAttributedString (mt, Theme.ErrorBubbleTextAttributes);

                var smt = amt.GetSize ();

                return new CGSize (smt.Width + 2 * hpad, smt.Height + 2 * hpad);
            }
        }

        public string MessageText {
            get {
                var mt = message.Text;
                if (string.IsNullOrWhiteSpace (mt))
                    return string.Empty;
                if (!message.Location.IsNull)
                    mt = $"Line {message.Location.Line:#,0}: {mt}";
                return mt;
            }
        }

        protected override void DrawDirtyRect (CGRect dirtyRect)
        {
            NativeColor.Clear.Set ();
            var drawRect = Bounds;
            drawRect.Intersect (dirtyRect);
            NativeGraphics.RectFill (drawRect);

            var bounds = Bounds;
            if (bounds.Width < bounds.Height)
                return;
            
            var mt = MessageText;

            var amt = new NSAttributedString (mt, Theme.ErrorBubbleTextAttributes);

            var smt = amt.GetSize ();
            bounds = new CGRect (bounds.X + bounds.Width - smt.Width - 2 * hpad, bounds.Y, smt.Width + 2 * hpad, bounds.Height);
            if (bounds.X < 0) {
                bounds.Width += bounds.X;
                bounds.X = 0;
            }
            if (bounds.Width < bounds.Height)
                return;

#if __IOS__
            var p = NativeBezierPath.FromRoundedRect (bounds, bounds.Height / 2);
#elif __MACOS__
            var p = NativeBezierPath.FromRoundedRect (bounds, bounds.Height / 2, bounds.Height / 2);
#endif
            Theme.ErrorBubbleBackgroundColor.ColorWithAlphaComponent (0.875f).SetFill ();
            p.Fill ();

            var abounds = bounds;
            abounds.Inflate (-hpad / 2, -1);
            var ctx = new NSStringDrawingContext ();
#if __MACOS__
            var bmt = amt.BoundingRectWithSize (new CGSize (abounds.Width, 1_000), NSStringDrawingOptions.UsesLineFragmentOrigin, ctx);
#elif __IOS__
            var bmt = amt.GetBoundingRect (new CGSize (abounds.Width, 1_000), NSStringDrawingOptions.UsesLineFragmentOrigin, ctx);
#endif
            //Console.WriteLine (bmt);
            nfloat scale = 1;
            if (bmt.Height > abounds.Height) {
                scale = abounds.Height / bmt.Height;
                var ammt = new NSMutableAttributedString (amt);
                ammt.SetAttributes (Theme.WithFontScale (Theme.FontScale * scale).ErrorBubbleTextAttributes, new NSRange (0, ammt.Length));
                amt = ammt;
#if __MACOS__
                bmt = amt.BoundingRectWithSize (new CGSize (abounds.Width, 1_000), NSStringDrawingOptions.UsesLineFragmentOrigin, ctx);
#elif __IOS__
                bmt = amt.GetBoundingRect (new CGSize (abounds.Width, 1_000), NSStringDrawingOptions.UsesLineFragmentOrigin, ctx);
#endif
            }
            amt.DrawInRect (new CGRect (
                bounds.X + bounds.Width / 2 - bmt.Width * scale / 2,
                bounds.Y + bounds.Height / 2 - bmt.Height * scale / 2,
                abounds.Width, 1_000));
        }
    }
}
