using JobTimecardTracker.WinForms.Models;
using JobTimecardTracker.WinForms.Services;

namespace JobTimecardTracker.WinForms;

public partial class MainForm : Form
{
    private readonly ApiClientService _apiClient;
    private List<JobDto> _jobs = new();

    // Controls - Jobs Tab
    private TextBox _txtJobNumber = null!;
    private ComboBox _cmbJobType = null!;
    private DateTimePicker _dtpJobDate = null!;
    private TextBox _txtCompanyName = null!;
    private TextBox _txtContactName = null!;
    private TextBox _txtTelephoneNumber = null!;
    private Button _btnCreateJob = null!;
    private Button _btnRefreshJobs = null!;
    private DataGridView _dgvJobs = null!;

    // Controls - Timecards Tab
    private TextBox _txtEmployee = null!;
    private DateTimePicker _dtpDateWorked = null!;
    private NumericUpDown _numHoursWorked = null!;
    private ComboBox _cmbAssociatedJob = null!;
    private Button _btnCreateTimecard = null!;
    private TextBox _txtFilterEmployee = null!;
    private DateTimePicker _dtpFilterDate = null!;
    private CheckBox _chkFilterByDate = null!;
    private Button _btnFilterTimecards = null!;
    private Button _btnClearFilters = null!;
    private DataGridView _dgvTimecards = null!;
    private Label _lblDailyHoursSummary = null!;

    private ToolStripStatusLabel _statusLabel = null!;

    public MainForm(string apiBaseUrl)
    {
        _apiClient = new ApiClientService(apiBaseUrl);
        InitializeComponent();
        Load += async (_, _) => await InitializeDataAsync();
    }

    private void InitializeComponent()
    {
        Text = "Job & Timecard Tracker (WinForms)";
        Size = new Size(1050, 680);
        StartPosition = FormStartPosition.CenterScreen;

        var tabControl = new TabControl { Dock = DockStyle.Fill };
        var tabJobs = new TabPage("Jobs");
        var tabTimecards = new TabPage("Timecards");

        BuildJobsTab(tabJobs);
        BuildTimecardsTab(tabTimecards);

        tabControl.TabPages.Add(tabJobs);
        tabControl.TabPages.Add(tabTimecards);

        var statusStrip = new StatusStrip();
        _statusLabel = new ToolStripStatusLabel("Ready");
        statusStrip.Items.Add(_statusLabel);

        Controls.Add(tabControl);
        Controls.Add(statusStrip);
    }

    private void BuildJobsTab(TabPage tab)
    {
        var grpCreate = new GroupBox
        {
            Text = "New Job",
            Location = new Point(15, 15),
            Size = new Size(995, 165),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        var lblJobNumber = new Label { Text = "Job Number:", Location = new Point(15, 30), AutoSize = true };
        _txtJobNumber = new TextBox { Location = new Point(120, 26), Width = 170 };

        var lblJobType = new Label { Text = "Job Type:", Location = new Point(315, 30), AutoSize = true };
        _cmbJobType = new ComboBox
        {
            Location = new Point(390, 26),
            Width = 150,
            DropDownStyle = ComboBoxStyle.DropDownList,
            DataSource = Enum.GetValues<JobType>()
        };

        var lblJobDate = new Label { Text = "Job Date:", Location = new Point(565, 30), AutoSize = true };
        _dtpJobDate = new DateTimePicker
        {
            Location = new Point(635, 26),
            Width = 150,
            Format = DateTimePickerFormat.Short
        };

        var lblCompany = new Label { Text = "Company Name:", Location = new Point(15, 70), AutoSize = true };
        _txtCompanyName = new TextBox { Location = new Point(120, 66), Width = 220 };

        var lblContact = new Label { Text = "Contact Name:", Location = new Point(360, 70), AutoSize = true };
        _txtContactName = new TextBox { Location = new Point(455, 66), Width = 180 };

        var lblPhone = new Label { Text = "Telephone:", Location = new Point(655, 70), AutoSize = true };
        _txtTelephoneNumber = new TextBox { Location = new Point(730, 66), Width = 170 };

        _btnCreateJob = new Button
        {
            Text = "Add Job",
            Location = new Point(120, 112),
            Width = 110,
            Height = 32
        };
        _btnCreateJob.Click += async (_, _) => await CreateJobAsync();

        _btnRefreshJobs = new Button
        {
            Text = "Refresh Jobs",
            Location = new Point(240, 112),
            Width = 120,
            Height = 32
        };
        _btnRefreshJobs.Click += async (_, _) => await LoadJobsAsync();

        grpCreate.Controls.AddRange(new Control[]
        {
            lblJobNumber, _txtJobNumber,
            lblJobType, _cmbJobType,
            lblJobDate, _dtpJobDate,
            lblCompany, _txtCompanyName,
            lblContact, _txtContactName,
            lblPhone, _txtTelephoneNumber,
            _btnCreateJob, _btnRefreshJobs
        });

        _dgvJobs = new DataGridView
        {
            Location = new Point(15, 195),
            Size = new Size(995, 385),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };

        tab.Controls.Add(grpCreate);
        tab.Controls.Add(_dgvJobs);
    }

    private void BuildTimecardsTab(TabPage tab)
    {
        var grpLog = new GroupBox
        {
            Text = "Log Timecard (Multiple jobs allowed per employee per day)",
            Location = new Point(15, 15),
            Size = new Size(995, 125),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        var lblEmployee = new Label { Text = "Employee:", Location = new Point(15, 32), AutoSize = true };
        _txtEmployee = new TextBox { Location = new Point(90, 28), Width = 180 };

        var lblDateWorked = new Label { Text = "Date Worked:", Location = new Point(285, 32), AutoSize = true };
        _dtpDateWorked = new DateTimePicker
        {
            Location = new Point(375, 28),
            Width = 135,
            Format = DateTimePickerFormat.Short
        };

        var lblHours = new Label { Text = "Hours Worked:", Location = new Point(525, 32), AutoSize = true };
        _numHoursWorked = new NumericUpDown
        {
            Location = new Point(620, 28),
            Width = 80,
            DecimalPlaces = 2,
            Increment = 0.25m,
            Minimum = 0.01m,
            Maximum = 24.00m,
            Value = 1.00m
        };

        var lblJob = new Label { Text = "Job:", Location = new Point(715, 32), AutoSize = true };
        _cmbAssociatedJob = new ComboBox
        {
            Location = new Point(750, 28),
            Width = 225,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        _btnCreateTimecard = new Button
        {
            Text = "Save Timecard",
            Location = new Point(90, 72),
            Width = 130,
            Height = 32
        };
        _btnCreateTimecard.Click += async (_, _) => await CreateTimecardAsync();

        _lblDailyHoursSummary = new Label
        {
            Text = "Total Hours Shown: 0.00",
            Location = new Point(240, 80),
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold)
        };

        grpLog.Controls.AddRange(new Control[]
        {
            lblEmployee, _txtEmployee,
            lblDateWorked, _dtpDateWorked,
            lblHours, _numHoursWorked,
            lblJob, _cmbAssociatedJob,
            _btnCreateTimecard, _lblDailyHoursSummary
        });

        var grpFilter = new GroupBox
        {
            Text = "Filter Timecards",
            Location = new Point(15, 148),
            Size = new Size(995, 68),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        var lblFilterEmp = new Label { Text = "Employee:", Location = new Point(15, 28), AutoSize = true };
        _txtFilterEmployee = new TextBox { Location = new Point(85, 24), Width = 170 };

        _chkFilterByDate = new CheckBox { Text = "Date:", Location = new Point(275, 26), AutoSize = true };
        _dtpFilterDate = new DateTimePicker
        {
            Location = new Point(335, 24),
            Width = 135,
            Format = DateTimePickerFormat.Short
        };

        _btnFilterTimecards = new Button { Text = "Apply Filter", Location = new Point(490, 22), Width = 105, Height = 28 };
        _btnFilterTimecards.Click += async (_, _) => await LoadTimecardsAsync();

        _btnClearFilters = new Button { Text = "Show All", Location = new Point(605, 22), Width = 95, Height = 28 };
        _btnClearFilters.Click += async (_, _) =>
        {
            _txtFilterEmployee.Clear();
            _chkFilterByDate.Checked = false;
            await LoadTimecardsAsync();
        };

        grpFilter.Controls.AddRange(new Control[]
        {
            lblFilterEmp, _txtFilterEmployee,
            _chkFilterByDate, _dtpFilterDate,
            _btnFilterTimecards, _btnClearFilters
        });

        _dgvTimecards = new DataGridView
        {
            Location = new Point(15, 224),
            Size = new Size(995, 356),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };

        tab.Controls.Add(grpLog);
        tab.Controls.Add(grpFilter);
        tab.Controls.Add(_dgvTimecards);
    }

    private async Task InitializeDataAsync()
    {
        await LoadJobsAsync();
        await LoadTimecardsAsync();
    }

    private async Task LoadJobsAsync()
    {
        try
        {
            SetBusy(true, "Loading jobs...");
            _jobs = await _apiClient.GetJobsAsync();

            _dgvJobs.DataSource = null;
            _dgvJobs.DataSource = _jobs.Select(j => new
            {
                j.Id,
                j.JobNumber,
                j.JobType,
                JobDate = j.JobDate.ToString("yyyy-MM-dd"),
                j.CompanyName,
                j.ContactName,
                j.TelephoneNumber
            }).ToList();

            var selectedJobId = _cmbAssociatedJob.SelectedValue as Guid?;
            _cmbAssociatedJob.DisplayMember = nameof(JobDto.DisplaySummary);
            _cmbAssociatedJob.ValueMember = nameof(JobDto.Id);
            _cmbAssociatedJob.DataSource = _jobs.ToList();

            if (selectedJobId.HasValue && _jobs.Any(j => j.Id == selectedJobId.Value))
            {
                _cmbAssociatedJob.SelectedValue = selectedJobId.Value;
            }

            _statusLabel.Text = $"Loaded {_jobs.Count} job(s).";
        }
        catch (Exception ex)
        {
            ShowError("Failed to load jobs", ex);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task CreateJobAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtJobNumber.Text) ||
            string.IsNullOrWhiteSpace(_txtCompanyName.Text) ||
            string.IsNullOrWhiteSpace(_txtContactName.Text) ||
            string.IsNullOrWhiteSpace(_txtTelephoneNumber.Text))
        {
            MessageBox.Show("Please fill in Job Number, Company Name, Contact Name, and Telephone Number.",
                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            SetBusy(true, "Saving job...");
            var dto = new CreateJobDto
            {
                JobNumber = _txtJobNumber.Text.Trim(),
                JobType = (JobType)_cmbJobType.SelectedItem!,
                JobDate = _dtpJobDate.Value.Date,
                CompanyName = _txtCompanyName.Text.Trim(),
                ContactName = _txtContactName.Text.Trim(),
                TelephoneNumber = _txtTelephoneNumber.Text.Trim()
            };

            await _apiClient.CreateJobAsync(dto);

            _txtJobNumber.Clear();
            _txtCompanyName.Clear();
            _txtContactName.Clear();
            _txtTelephoneNumber.Clear();

            await LoadJobsAsync();
            _statusLabel.Text = "Job saved.";
        }
        catch (Exception ex)
        {
            ShowError("Failed to create job", ex);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task LoadTimecardsAsync()
    {
        try
        {
            SetBusy(true, "Loading timecards...");
            var employeeFilter = string.IsNullOrWhiteSpace(_txtFilterEmployee.Text)
                ? null
                : _txtFilterEmployee.Text.Trim();
            DateTime? dateFilter = _chkFilterByDate.Checked ? _dtpFilterDate.Value.Date : null;

            var timecards = await _apiClient.GetTimecardsAsync(employeeFilter, dateFilter);

            _dgvTimecards.DataSource = null;
            _dgvTimecards.DataSource = timecards.Select(t => new
            {
                t.Id,
                t.Employee,
                DateWorked = t.DateWorked.ToString("yyyy-MM-dd"),
                t.HoursWorked,
                t.JobNumber,
                t.CompanyName
            }).ToList();

            var totalHours = timecards.Sum(t => t.HoursWorked);
            _lblDailyHoursSummary.Text = $"Total Hours Shown: {totalHours:0.00}";
            _statusLabel.Text = $"Loaded {timecards.Count} timecard(s).";
        }
        catch (Exception ex)
        {
            ShowError("Failed to load timecards", ex);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task CreateTimecardAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtEmployee.Text))
        {
            MessageBox.Show("Please enter an Employee name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_cmbAssociatedJob.SelectedValue is not Guid jobId || jobId == Guid.Empty)
        {
            MessageBox.Show("Please select an Associated Job (create a job first if the list is empty).",
                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            SetBusy(true, "Saving timecard...");
            var dto = new CreateTimecardDto
            {
                Employee = _txtEmployee.Text.Trim(),
                DateWorked = _dtpDateWorked.Value.Date,
                HoursWorked = _numHoursWorked.Value,
                JobId = jobId
            };

            await _apiClient.CreateTimecardAsync(dto);
            await LoadTimecardsAsync();
            _statusLabel.Text = "Timecard saved.";
        }
        catch (Exception ex)
        {
            ShowError("Failed to create timecard", ex);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void SetBusy(bool isBusy, string? statusText = null)
    {
        UseWaitCursor = isBusy;
        _btnCreateJob.Enabled = !isBusy;
        _btnRefreshJobs.Enabled = !isBusy;
        _btnCreateTimecard.Enabled = !isBusy;
        _btnFilterTimecards.Enabled = !isBusy;
        _btnClearFilters.Enabled = !isBusy;
        if (!string.IsNullOrWhiteSpace(statusText))
        {
            _statusLabel.Text = statusText;
        }
    }

    private static void ShowError(string title, Exception ex)
    {
        MessageBox.Show($"{title}:\n\n{ex.Message}", title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}