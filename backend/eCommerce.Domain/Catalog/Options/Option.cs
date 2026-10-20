namespace eCommerce.Domain.Catalog.Options
{
    public class Option
    {
        private readonly List<OptionValue> _values = [];
        public int Id { get; private set; }
        public string Name { get; private set; }
        public IReadOnlyCollection<OptionValue> Values => _values;
        public Option(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Option name cannot be empty", nameof(name));

            Name = name.Trim();
        }

        public void AddValue(OptionValue value)
        {
            if (_values.Any(v => string.Equals(v.Value, value.Value, StringComparison.OrdinalIgnoreCase))) 
                throw new InvalidOperationException(
                    $"Option value {value.Value} already belongs to this option");
            _values.Add(value);
        }
        public void RemoveValue(OptionValue value)
        {
            _values.Remove(value);
        }
        
        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Option name cannot be empty", nameof(name));

            Name = name.Trim();
        }

        public void RenameValue(int valueId, string newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue)) throw new ArgumentException(
                    "Option value cannot be empty", nameof(newValue));

            var value = _values.FirstOrDefault(v => v.Id == valueId)
                ?? throw new InvalidOperationException(
                    $"Option value with ID {valueId} does not belong to this option");

            var trimmedValue = newValue.Trim();

            if (_values.Any(v => v.Id != valueId && 
                string.Equals(v.Value, trimmedValue, StringComparison.OrdinalIgnoreCase)))
            
                throw new InvalidOperationException(
                    $"Option value {trimmedValue} already belongs to this option");
            
            value.Rename(trimmedValue);
        }
    }
}