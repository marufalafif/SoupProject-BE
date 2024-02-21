namespace SoupProject.Email
{
    public class EmailModel
    {
        public List<string> To { get; }
        public List<string> Bcc { get; }
        public List<string> Cc { get; }


        // Content
        public string Subject { get; }
        public string? Body { get; }

        public EmailModel(List<string> to,
                          string subject,
                          string? body = null,
                          List<string>? bcc = null,
                          List<string>? cc = null)
        {
            // Receiver
            To = to;
            Bcc = bcc ?? new List<string>();
            Cc = cc ?? new List<string>();

            // Content
            Subject = subject;
            Body = body;
        }
    }
}
