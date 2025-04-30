﻿using System;

#if __IOS__
using UIKit;
using NativeColor = UIKit.UIColor;
using NativeFont = UIKit.UIFont;
using NativeGraphics = UIKit.UIGraphics;
using NativeStringAttributes = UIKit.UIStringAttributes;
using NativeTextAlignment = UIKit.UITextAlignment;
using NativeTextView = UIKit.UITextView;
using NativeLineBreakMode = UIKit.UILineBreakMode;
#elif __MACOS__
using AppKit;
using NativeColor = AppKit.NSColor;
using NativeFont = AppKit.NSFont;
using NativeGraphics = AppKit.NSGraphics;
using NativeStringAttributes = AppKit.NSStringAttributes;
using NativeTextAlignment = AppKit.NSTextAlignment;
using NativeTextView = AppKit.NSTextView;
using NativeLineBreakMode = AppKit.NSLineBreakMode;
#endif

using Foundation;
using CoreGraphics;

namespace CLanguage.Editor
{
    static class Extensions
    {
        public static string GetIndent (this string line)
        {
            var e = 0;
            while (e < line.Length && char.IsWhiteSpace (line[e]))
                e++;
            if (e == 0)
                return string.Empty;
            return line.Substring (0, e);
        }

        public static string Localize (this string english)
        {
            return NSBundle.MainBundle.GetLocalizedString(key: english);
        }

#if __IOS__
        public static NativeColor Rgb (int r, int g, int b) => NativeColor.FromRGB (r, g, b);
        public static NativeColor Gray (int g) => NativeColor.FromWhiteAlpha (g / ((nfloat)255), 1);
        public static NativeColor ColorWithAlphaComponent (this NativeColor color, nfloat alpha) => color.ColorWithAlpha (alpha);
        public static void Set (this NativeColor color) => color.SetColor ();
        public static CGContext NativeGraphicsCGContext => NativeGraphics.GetCurrentContext ();
        public static NativeFont Font (string name, int size) => NativeFont.FromName (name, size);
        public static string GetFontName (this NativeFont f) => f.Name;
        public static NSUnderlineStyle ToKit (this NSUnderlineStyle s) => s;
        public static NativeTextAlignment TextAlignmentRight = NativeTextAlignment.Right;
        public static CGSize StringSize (this string text, NSDictionary attributes) =>
            new NSAttributedString (text, attributes).Size;
        public static CGSize GetSize (this NSAttributedString text) =>
            text.Size;
        public static void DrawInRect (this string text, CGRect rect, NSDictionary attributes) =>
            new NSAttributedString (text, attributes).DrawString (rect);
        public static void DrawInRect (this NSAttributedString text, CGRect rect) =>
            text.DrawString (rect);
        public static NativeLineBreakMode NativeLineBreakModeClipping => NativeLineBreakMode.Clip;
#else
        public static NativeColor Rgb (int r, int g, int b) => NativeColor.FromRgb (r, g, b);
        public static NativeColor Gray (int g) => NativeColor.FromWhite (g / ((nfloat)255), 1);
        public static CGContext NativeGraphicsCGContext => NSGraphicsContext.CurrentContext.CGContext;
        public static NativeFont Font (string name, int size) => NativeFont.FromFontName (name, size);
        public static string GetFontName (this NativeFont f) => f.FontName;
        public static int ToKit (this NSUnderlineStyle s) => (int)s;
        public static NativeTextAlignment TextAlignmentRight = NativeTextAlignment.Right;
        public static NSRange CharacterRangeForGlyphRange (this NSLayoutManager layoutManager, NSRange glyphRange) => layoutManager.GetCharacterRange (glyphRange);
        public static NSRange GlyphRangeForCharacterRange (this NSLayoutManager layoutManager, NSRange charRange) => layoutManager.GetGlyphRange (charRange);
        public static NativeLineBreakMode NativeLineBreakModeClipping => NativeLineBreakMode.Clipping;
#endif
    }
}
