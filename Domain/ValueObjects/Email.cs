namespace Domain.ValueObjects
{
    public class Email
    {
        public string Address { get; private set; }

        public Email(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Email address cannot be empty.");

            Address = address;
        }

    }
}
