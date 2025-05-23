using System.Windows;
using System.Windows.Controls;

namespace NotoNote.Utilities;
public class PasswordBoxHelper : DependencyObject
{
    public static readonly DependencyProperty AttachmentProperty
        = DependencyProperty.RegisterAttached(
            "Attachment",
            typeof(bool),
            typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(false, AttachmentProperty_Changed));

    public static readonly DependencyProperty PasswordProperty
        = DependencyProperty.RegisterAttached(
            "Password",
            typeof(string),
            typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static bool GetAttachment(DependencyObject dp)
    {
        return (bool)dp.GetValue(AttachmentProperty);
    }

    public static void SetAttachment(DependencyObject dp, bool value)
    {
        dp.SetValue(AttachmentProperty, value);
    }

    public static string GetPassword(DependencyObject dp)
    {
        return (string)dp.GetValue(PasswordProperty);
    }

    public static void SetPassword(DependencyObject dp, string value)
    {
        dp.SetValue(PasswordProperty, value);
    }

    private static void AttachmentProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (!(sender is PasswordBox passwordBox)) return;

        if ((bool)e.NewValue)
        {
            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }
    }

    private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        var passwordBox = (PasswordBox)sender;
        SetPassword(passwordBox, passwordBox.Password);
    }
}