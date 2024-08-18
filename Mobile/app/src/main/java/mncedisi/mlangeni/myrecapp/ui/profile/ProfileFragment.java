package mncedisi.mlangeni.myrecapp.ui.profile;

import android.app.ProgressDialog;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.os.Handler;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.fragment.app.Fragment;

import com.google.android.material.snackbar.Snackbar;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;

import mncedisi.mlangeni.myrecapp.R;
import mncedisi.mlangeni.myrecapp.adapters.CreateConnection;
import mncedisi.mlangeni.myrecapp.adapters.UserData;
import mncedisi.mlangeni.myrecapp.data.User;

public class ProfileFragment extends Fragment {
    TextView txtProfileNames, txtRoles;
    Button btnManageProfile;
    private Connection connection;
    Handler handler = new Handler();
    private User logged_user;
    private UserData data;
    private ResultSet user, roleset;
    ProgressDialog dialog;
    CreateConnection createConnection;
    String role;

    public View onCreateView(LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {
        createConnection = new CreateConnection(getActivity());
        dialog = ProgressDialog.show(getActivity(),"Loading profile", "Please wait...");
        return inflater.inflate(R.layout.fragment_profile, container, false);
    }

    private void onManageProfileClick(View view) {
        View diag_view = LayoutInflater.from(getActivity()).inflate(R.layout.activity_edit_user, null);
        AlertDialog builder = new AlertDialog.Builder(getActivity()).create();

        EditText txtEditFirstName = diag_view.findViewById(R.id.txtEditFirstName),
                txtEditLastName = diag_view.findViewById(R.id.txtEditLastName),
                txtEditStudentNumber = diag_view.findViewById(R.id.txtEditStudent),
                txtEditIdNumber = diag_view.findViewById(R.id.txtEditIDNumber),
                txtEditEmail = diag_view.findViewById(R.id.txtEditEmail);

        builder.setView(diag_view);
        builder.setCanceledOnTouchOutside(true);
        txtEditFirstName.setText(logged_user.getFirstName());
        txtEditLastName.setText(logged_user.getLastName());
        txtEditStudentNumber.setText(logged_user.getStudentNumber());
        txtEditIdNumber.setText(logged_user.getIdNumber());
        txtEditEmail.setText(logged_user.getNormalizedEmail());

        if(logged_user.getIdNumber() == null)
            txtEditIdNumber.setVisibility(View.GONE);
        if(logged_user.getStudentNumber() == null)
            txtEditStudentNumber.setVisibility(View.GONE);

        diag_view.findViewById(R.id.btnUpdateUser).setOnClickListener(v3 -> {

            String sqlQuery = "update AspNetUsers set " +
                    (logged_user.getStudentNumber() == null ? "" : " StudentStaffNumber = '" + txtEditStudentNumber.getText() + "', ") +
                    (logged_user.getIdNumber() == null ? "" : " IdPassportNumber = '" + txtEditIdNumber.getText() + "' ") +
                    " FirstName = '" + txtEditFirstName.getText() + "'," +
                    " LastName = '" + txtEditLastName.getText() + "' " +
                    "where NormalizedEmail = '" + txtEditEmail.getText() + "' ";

            try {
                Statement statement = connection.createStatement();

                int numColsAffected = statement.executeUpdate(sqlQuery);
                if (numColsAffected > 0) {
                    Toast.makeText(getActivity(), "User updated successfully.", Toast.LENGTH_LONG).show();
                    builder.dismiss();
                } else {
                    Toast.makeText(getActivity(),
                            "Couldn't update users.", Toast.LENGTH_LONG).show();
                    builder.dismiss();
                }
            } catch (Exception e) {
                Toast.makeText(getActivity(),
                        "Database error. Please contact system administrator.", Toast.LENGTH_LONG).show();
                builder.dismiss();
                e.printStackTrace();
            }
        });
        diag_view.findViewById(R.id.btnGoBackToUser).setOnClickListener(v3 -> {
            builder.dismiss();
        });
        builder.show();
    }


    @Override
    public void onStart() {
        super.onStart();
        txtProfileNames = getActivity().findViewById(R.id.txtProfileNames);
        txtRoles = getActivity().findViewById(R.id.txtRoles);
        btnManageProfile = getActivity().findViewById(R.id.btnManageProfile);

        new Thread(() -> {
            this.connection = createConnection.getConnection();
            handler.post(new Runnable() {
                @Override
                public void run() {
                    data = (UserData) requireActivity().getIntent().getSerializableExtra("data");
                    user = createConnection.getData("SELECT * FROM AspNetUsers where NormalizedEmail = '" + data.getUserEmail().toUpperCase() + "';");

                    logged_user = User.createUserFromResultSet(user);
                    txtProfileNames.setText(logged_user.getFirstName().toUpperCase()
                            + " " + logged_user.getLastName().toUpperCase());

                    roleset = createConnection.getData("select Name from AspNetUserRoles join AspNetUsers " +
                            "on AspNetUsers.Id = UserId " +
                            "join AspNetRoles on " +
                            "AspNetRoles.Id = AspNetUserRoles.RoleId " +
                            "where AspNetUsers.Id = '" + logged_user.getId() + "'");
                    role = "User";
                    try {
                        while (roleset.next()) role = roleset.getString(1);
                    } catch (SQLException e) {
                        e.printStackTrace();
                    }
                    txtRoles.setText(role);
                    btnManageProfile.setVisibility(View.VISIBLE);
                    dialog.dismiss();
                }
            });
        }).start();

        btnManageProfile.setOnClickListener(this::onManageProfileClick);
    }

}
