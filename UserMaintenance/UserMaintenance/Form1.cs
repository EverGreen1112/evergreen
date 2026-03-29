namespace UserMaintenance
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            lblLastName.Text = Resource1.LastName;
            lblFirstName.Text = Resource1.FirstName;
            btnAdd.Text = Resource1.Add;
        }
    }
}
