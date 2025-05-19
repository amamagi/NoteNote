namespace NotoNote.Models;
public record Hotkey : RecordWithValidation
{
    public Hotkey(Keys key, Keys modifiers)
    {
        Key = key;
        Modifiers = modifiers;
    }

    protected override void Validate()
    {
        if ((Key & Keys.Modifiers) != 0) throw new ArgumentException();
        if ((Modifiers & ~Keys.Modifiers) != 0) throw new ArgumentException();
    }

    public Keys Key { get; }
    public Keys Modifiers { get; }

    public override string ToString()
    {
        var key = Key.ToString();
        var modifiers = Modifiers.ToString().Replace("Control", "Ctrl").Replace(", ", "+");
        return $"{modifiers}+{key}";
    }
}