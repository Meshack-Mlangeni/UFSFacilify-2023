package mncedisi.mlangeni.myrecapp.ui.book;

import androidx.appcompat.app.AppCompatActivity;

import android.app.DatePickerDialog;
import android.app.TimePickerDialog;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.DatePicker;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.TimePicker;

import com.google.android.material.snackbar.Snackbar;

import net.sourceforge.jtds.jdbc.DateTime;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Timestamp;
import java.text.SimpleDateFormat;
import java.time.LocalDate;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Calendar;
import java.util.Date;
import java.util.Random;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

import mncedisi.mlangeni.myrecapp.R;
import mncedisi.mlangeni.myrecapp.adapters.CreateConnection;
import mncedisi.mlangeni.myrecapp.adapters.UserData;
import mncedisi.mlangeni.myrecapp.data.Category;
import mncedisi.mlangeni.myrecapp.data.Facility;

public class MakeBookingActivity extends AppCompatActivity {

    Facility facility;
    ImageButton btnDatePicker, btnTimePicker;
    Button btnPayAtEntrance, btnPayOnline;
    TextView txtDate, txtTime;
    AutoCompleteTextView spnrCategoryType;
    private int mYear, mMonth, mDay, mHour, mMinute;
    Date date;
    final Calendar c = Calendar.getInstance();
    private String sSelectedValue;
    private Category[] categories;
    private Connection connection;
    CreateConnection createConnection;

    private String UserEmail;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.make_booking);
        date = new Date();
        connection = null;
        createConnection = new CreateConnection(this);
        connection = createConnection.getConnection();
        sSelectedValue = "";
        btnDatePicker = findViewById(R.id.btnSelectDate);
        btnTimePicker = findViewById(R.id.btnSelectTime);
        txtDate = findViewById(R.id.txtDateDialog);
        txtTime = findViewById(R.id.txtTimeDialog);
        btnPayAtEntrance = findViewById(R.id.btnPayAtEntrance);
        spnrCategoryType = findViewById(R.id.spnrCategoryType);
        btnPayOnline = findViewById(R.id.btnPayOnline);

        btnDatePicker.setOnClickListener(view -> onDateButtonClick(view));
        btnTimePicker.setOnClickListener(view -> onTimeButtonClick(view));
        btnPayAtEntrance.setOnClickListener(view -> onPayAtEntClick(view));
        btnPayOnline.setOnClickListener(view -> onPayOnline(view));

        facility = (Facility) getIntent().getSerializableExtra("facility");
        categories = ((ArrayList<Category>) getIntent().getSerializableExtra("categories"))
                .stream().filter(cat -> cat.getFacilityId() == facility.getFacilityId()).toArray(Category[]::new);

        spnrCategoryType.setAdapter(
                new ArrayAdapter<>(getApplicationContext(), android.R.layout.simple_spinner_dropdown_item,
                        Arrays.stream(categories).map(a -> a.getCategoryName()).toArray(String[]::new)));


        ((TextView) findViewById(R.id.txtBookingDetails)).setText(
                "User has requested to make a booking for " + facility.getName() + ". The facility costs R" + facility.getPrice() + ",00."
        );
        UserEmail = getIntent().getStringExtra("userEmail");
    }

    private void onDateButtonClick(View view) {
        mYear = c.get(Calendar.YEAR);
        mMonth = c.get(Calendar.MONTH);
        mDay = c.get(Calendar.DAY_OF_MONTH);
        mHour = c.get(Calendar.HOUR_OF_DAY);
        mMinute = c.get(Calendar.MINUTE);

        DatePickerDialog datePickerDialog = new DatePickerDialog(this,
                new DatePickerDialog.OnDateSetListener() {
                    @Override
                    public void onDateSet(DatePicker view, int year,
                                          int monthOfYear, int dayOfMonth) {

                        c.set(year, monthOfYear + 1, dayOfMonth);
                        txtDate.setText(dayOfMonth + "-" + (monthOfYear + 1) + "-" + year);
                    }
                }, mYear, mMonth, mDay);
        datePickerDialog.show();
    }

    private void onTimeButtonClick(View view) {

        mHour = c.get(Calendar.HOUR_OF_DAY);
        mMinute = c.get(Calendar.MINUTE);
        mYear = c.get(Calendar.YEAR);
        mMonth = c.get(Calendar.MONTH);
        mDay = c.get(Calendar.DAY_OF_MONTH);

        TimePickerDialog timePickerDialog = new TimePickerDialog(this,
                new TimePickerDialog.OnTimeSetListener() {

                    @Override
                    public void onTimeSet(TimePicker view, int hourOfDay,
                                          int minute) {
                        c.set(mYear, mMonth, mDay, hourOfDay, minute);
                        txtTime.setText(hourOfDay + ":" + minute);
                    }
                }, mHour, mMinute, false);
        timePickerDialog.show();
    }

    Category _category;

    private void onPayAtEntClick(View view) {
        sSelectedValue = spnrCategoryType.getText().toString();
        if (sSelectedValue != "" && !anyEmpty(txtDate, txtTime)) {
            for (Category cat : categories)
                if (sSelectedValue.equals(cat.getCategoryName())) {
                    _category = cat;
                    break;
                }

            createBooking(UserEmail, facility, _category, 0);
        } else createMessage("Please fill in all the required values.");
    }

    private void onPayOnline(View view) {
        // if()
    }

    private Snackbar createMessage(CharSequence text) {
        return Snackbar.make(findViewById(R.id.splash_view), text, Snackbar.LENGTH_INDEFINITE);
    }

    public boolean anyEmpty(TextView... controls) {
        return Arrays.stream(controls).anyMatch(c -> !(c.getText().toString().length() > 0));
    }

    private void createBooking(String userEmail, Facility facility, Category category, int isApproved) {
        String possibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        Random random = new Random();
        String date_start = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format(c.getTime());
        c.add(Calendar.MINUTE, 30);
        String date_end = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format(c.getTime());
        String bookingPass = "mob-" + IntStream.range(0, 5)
                .mapToObj(i -> possibleChars.charAt(random.nextInt(possibleChars.length())))
                .map(Object::toString)
                .collect(Collectors.joining()) + "-" + userEmail.charAt(0);
        ResultSet resultSet = null;
        String sqlQuery = "INSERT INTO Booking (DateStart, DateEnd ,BookingPass ,IsValid ,Approved,UserEmail,FacilityId ,CategoryId)" +
                " VALUES ( '" + date_start + "','" + date_end + "','" + bookingPass + "'," + 1 + "," + isApproved + ",'" + userEmail + "'," +
                facility.getFacilityId() + "," + category.getCategoryId() + ")";
        try {
            Statement statement = connection.createStatement();

            int numColsAffected = statement.executeUpdate(sqlQuery);
            Intent intent;
            if (numColsAffected > 0) {
                intent = new Intent(this, SuccessActivity.class);
                intent.putExtra("pass", bookingPass);
            } else {
                intent = new Intent(this, CancelActivity.class);
            }
            startActivity(intent);
            finish();

        } catch (Exception e) {
            e.printStackTrace();
        }

    }
}
