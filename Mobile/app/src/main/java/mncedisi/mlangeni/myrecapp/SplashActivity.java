package mncedisi.mlangeni.myrecapp;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Bundle;
import android.os.Handler;
import android.os.StrictMode;
import android.view.Gravity;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.RadioButton;
import android.widget.RadioGroup;
import android.widget.ScrollView;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;

import mncedisi.mlangeni.myrecapp.adapters.CreateConnection;
import mncedisi.mlangeni.myrecapp.adapters.UserData;
import mncedisi.mlangeni.myrecapp.ui.book.CancelActivity;
import mncedisi.mlangeni.myrecapp.ui.book.SuccessActivity;

import com.google.android.material.snackbar.Snackbar;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.text.SimpleDateFormat;
import java.time.LocalDate;
import java.util.Arrays;
import java.util.Calendar;
import java.util.Date;
import java.util.Objects;
import java.util.UUID;


public class SplashActivity extends AppCompatActivity {


    private LinearLayout loginLayout, noConnectionLayout, logo_layout;
    private ScrollView registerLayout;
    Handler handler= new Handler();
    Button btnRetry, btnGoToRegister, btnLogin, btnGoToLogin, btnRegister;
    EditText txtLoginEmail, txtLoginPassword, txtRegisterFName, txtRegisterLName, txtRegisterPass,
            txtRegisterCPass, txtRegisterIdNumber, txtRegisterStudentStaff, txtRegisterEmail;
    private Connection connection;
    CreateConnection createConnection;

    // TODO: 2023/10/16 Check if students or visitors who are registering and fix quering the database 
    RadioGroup rdoGroup;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_splash);
        connection = null;

        logo_layout = findViewById(R.id.logo_layout);
        loginLayout = findViewById(R.id.login_reg);
        logo_layout.setVerticalGravity(Gravity.CENTER_VERTICAL);
        registerLayout = findViewById(R.id.registerLayout);
        btnRetry = findViewById(R.id.btnRetry);
        btnLogin = findViewById(R.id.btnLogin);
        btnGoToRegister = findViewById(R.id.btnLoginRegister);
        btnGoToLogin = findViewById(R.id.btnGoToLogin);
        btnRegister = findViewById(R.id.btnRegister);
        noConnectionLayout = findViewById(R.id.no_connection);
        txtLoginEmail = findViewById(R.id.txtLoginEmail);
        txtLoginPassword = findViewById(R.id.txtLoginPass);
        txtRegisterFName = findViewById(R.id.txtRegisterFirstName);
        txtRegisterLName = findViewById(R.id.txtRegisterLastName);
        txtRegisterIdNumber = findViewById(R.id.txtRegisterIdNumber);
        txtRegisterIdNumber.setVisibility(View.GONE);
        txtRegisterStudentStaff = findViewById(R.id.txtRegisterStudentStaffNumber);
        txtRegisterPass = findViewById(R.id.txtRegisterPassword);
        txtRegisterCPass = findViewById(R.id.txtConfirmPassword);
        txtRegisterEmail = findViewById(R.id.txtRegisterEmail);
        rdoGroup = findViewById(R.id.rdoGroup);

        rdoGroup.setOnCheckedChangeListener(new RadioGroup.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(RadioGroup radioGroup, int i) {
                if (("Visitor".equals(((RadioButton) findViewById(radioGroup.getCheckedRadioButtonId())).getText().toString())))
                    txtRegisterIdNumber.setVisibility(View.VISIBLE);
                else
                    txtRegisterIdNumber.setVisibility(View.GONE);

            }
        });

        createConnection = new CreateConnection(this);

        NetworkInfo activeNetwork = ((ConnectivityManager) this.getSystemService(CONNECTIVITY_SERVICE)).getActiveNetworkInfo();
        if (activeNetwork != null && activeNetwork.isConnectedOrConnecting()) startProcess();
        else noInternetConnectivity("Mobile data is switched off.");
    }

    private void noInternetConnectivity(CharSequence message) {
        btnRetry.setOnClickListener(view -> this.recreate());
        ((TextView) findViewById(R.id.txtNoBookings))
                .setText(message);
        createMessage(message).show();
        loginLayout.setVisibility(View.GONE);
        noConnectionLayout.setVisibility(View.VISIBLE);
    }

    private void goToHomePage(UserData data) {
        Intent home = new Intent(SplashActivity.this, MainActivity.class);
        home.putExtra("data", (Serializable) data);
        startActivity(home);
        finish();
    }

    private void startProcess() {

        Snackbar dialog = createMessage("Connecting to server, please wait...");
        dialog.show();
        new Thread(() -> {
            connection = createConnection.getConnection();
            handler.post(() -> {
                dialog.dismiss();
                if (connection != null) showLoginPage(connection);
                else noInternetConnectivity
                        ("Failed to establish connection, please check internet connection or contact admin.");
            });
        }).start();
    }

    private Snackbar createMessage(CharSequence text) {
        return Snackbar.make(findViewById(R.id.splash_view), text, Snackbar.LENGTH_INDEFINITE);
    }

    public boolean anyEmpty(EditText... controls) {
        return Arrays.stream(controls).anyMatch(c -> !(c.getText().toString().length() > 0));
    }

    public void displayErrorIfEmpty(EditText... controls) {
        for (EditText control : Arrays.stream(controls)
                .filter(c -> !(c.getText().toString().length() > 0)).toArray(EditText[]::new))
            control.setError("Please fill in value");
    }

    private void showLoginPage(Connection connection) {
        btnGoToRegister.setOnClickListener(view -> showRegisterPage(connection));
        btnLogin.setOnClickListener(view -> {
            if (!anyEmpty(txtLoginEmail, txtLoginPassword)) {
                ResultSet resultSet = getData(connection,
                        "SELECT * FROM AspNetUsers");
                String loginPassword = "", loginEmail = "", userId = "";

                try {
                    while (resultSet.next()) {
                        if (resultSet.getString(12).equals(txtLoginEmail.getText().toString().toUpperCase())) {
                            userId = resultSet.getString(1);
                            loginEmail = txtLoginEmail.getText().toString();
                            loginPassword = resultSet.getString(8);
                            break;
                        }
                    }
                } catch (Exception e) {
                    e.printStackTrace();
                }

                if ("".equals(loginEmail)) createMessage("Could not find user");
                if (loginEmail.equals(txtLoginEmail.getText().toString()) &&
                        loginPassword.equals(txtLoginPassword.getText().toString()))
                    goToHomePage(new UserData(txtLoginEmail.getText().toString(), userId));
                else createMessage("Invalid email or password")
                        .setDuration(Snackbar.LENGTH_LONG)
                        .setBackgroundTint(getResources().getColor(R.color.purple_200))
                        .show();


            } else displayErrorIfEmpty(txtLoginEmail, txtLoginPassword);
        });
        registerLayout.setVisibility(View.GONE);
        noConnectionLayout.setVisibility(View.GONE);
        loginLayout.setVisibility(View.VISIBLE);
    }


    private void showRegisterPage(Connection connection) {
        btnGoToLogin.setOnClickListener(view -> showLoginPage(connection));
        btnRegister.setOnClickListener(view -> {
            if (!anyEmpty(txtRegisterFName, txtRegisterLName,
                    txtRegisterStudentStaff, txtRegisterEmail, txtRegisterPass, txtRegisterCPass)) {
                if (!txtRegisterPass.getText().equals(txtRegisterCPass.getText())) {

                    ResultSet resultSet = getData(connection,
                            "SELECT NormalizedEmail FROM AspNetUsers");
                    String loginEmail = "";
                    boolean userExists = false;
                    try {
                        while (resultSet.next()) {
                            if (resultSet.getString(1).equals(txtLoginEmail.getText().toString().toUpperCase())) {
                                userExists = true;
                                break;
                            }
                        }
                    } catch (Exception e) {
                        createMessage("An error occurred, please contact administrator");
                        e.printStackTrace();
                    }

                    if(userExists){
                        createMessage("User already exists.");
                    }else {
                        UUID uuid = UUID.randomUUID();
                        String userId = uuid.toString();
                        Calendar c = Calendar.getInstance();

// TODO: 2023/10/23 check if user corresponds
                        String sqlQuery = "INSERT INTO AspNetUsers (Id, FirstName ,LastName ,IdPassportNumber ,DateJoined," +
                                " DateModified , StudentStaffNumber,MobilePassword ,UserName, EmailConfirmed, PhoneNumberConfirmed," +
                                "TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, NormalizedEmail)" +
                                " VALUES ('" + userId + "', '" + txtRegisterFName.getText().toString()
                                + "','" + txtRegisterLName.getText().toString()
                                + "'," + ("".equals(txtRegisterIdNumber.getText().toString()) ? null : "'" + txtRegisterIdNumber.getText().toString() + "'")
                                + ",'" + (new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format(c.getTime()))
                                + "','" + (new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format(c.getTime()))
                                + "'," + ("".equals(txtRegisterStudentStaff.getText().toString()) ? null : "'" + txtRegisterStudentStaff.getText().toString() + "'")
                                + ",'" + txtRegisterPass.getText().toString()
                                + "','" + txtRegisterLName.getText().toString() +
                                ("".equals(txtRegisterIdNumber.getText().toString()) ? txtRegisterStudentStaff.getText().toString() : txtRegisterIdNumber.getText().toString())
                                + "'," + 0 + "," + 0 + "," + 0 + "," + 1 + "," + 0 + ",'" + txtRegisterEmail.getText().toString()
                                + "','" + txtRegisterEmail.getText().toString().toUpperCase() + "');" +
                                " insert into AspNetUserRoles(UserId, RoleId) " +
                                " values('" + userId + "', (select top(1) Id from AspNetRoles " +
                                "where Name = 'User'))";
                        try {
                            Statement statement = connection.createStatement();

                            int numColsAffected = statement.executeUpdate(sqlQuery);
                            if (numColsAffected > 0) {
                                goToHomePage(new UserData(txtRegisterEmail.getText().toString(), userId));
                            } else {
                                createMessage("Registration failed, pleas contact system administrator.");
                            }
                        } catch (Exception e) {
                            e.printStackTrace();
                        }
                    }
                } else createMessage("Passwords must match")
                        .setDuration(Snackbar.LENGTH_LONG)
                        .setBackgroundTint(getResources().getColor(R.color.success))
                        .show();
            } else displayErrorIfEmpty(txtRegisterFName, txtRegisterLName, txtRegisterIdNumber,
                    txtRegisterStudentStaff, txtRegisterEmail, txtRegisterPass, txtRegisterCPass);
        });
        loginLayout.setVisibility(View.GONE);
        noConnectionLayout.setVisibility(View.GONE);
        registerLayout.setVisibility(View.VISIBLE);
    }

    private ResultSet getData(Connection connection, String query) {
        ResultSet resultSet = null;
        try {
            Statement statement = connection.createStatement();
            resultSet = statement.executeQuery(query);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return resultSet;
    }
}
