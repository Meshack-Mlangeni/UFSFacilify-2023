package mncedisi.mlangeni.myrecapp.adapters;

import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;

import mncedisi.mlangeni.myrecapp.MainActivity;
import mncedisi.mlangeni.myrecapp.R;
import mncedisi.mlangeni.myrecapp.SplashActivity;
import mncedisi.mlangeni.myrecapp.data.Category;
import mncedisi.mlangeni.myrecapp.data.Facility;
import mncedisi.mlangeni.myrecapp.data.User;
import mncedisi.mlangeni.myrecapp.ui.book.EditFacilityActivity;
import mncedisi.mlangeni.myrecapp.ui.book.MakeBookingActivity;

import static androidx.core.content.ContextCompat.startActivity;

import androidx.appcompat.app.AlertDialog;

import com.google.android.material.snackbar.Snackbar;

public class FacilityAdapter extends BaseAdapter {

    private final Context context;
    private ArrayList<Facility> facilities;
    private UserData userData;
    private ArrayList<Category> categories;
    private String userRole;
    private Connection connection;

    public FacilityAdapter(Context context, ArrayList<Facility> facilities,
                           ArrayList<Category> categories, UserData userData, String role, Connection _con) {
        this.context = context;
        this.facilities = facilities;
        this.categories = categories;
        this.userData = userData;
        this.userRole = role;
        connection = _con;
    }

    @Override
    public int getCount() {
        return facilities.size();
    }


    @Override
    public Object getItem(int i) {
        return facilities.get(i);
    }

    @Override
    public long getItemId(int i) {
        return i;
    }

    @Override
    public View getView(int i, View view, ViewGroup viewGroup) {
        View myView = View.inflate(context, R.layout.app_facility_item, null);
        ((TextView) myView.findViewById(R.id.txtPrice)).setText("R" + facilities.get(i).getPrice() + ".00");
        ((TextView) myView.findViewById(R.id.txtSpaceAvailable)).setText("Space available: " + facilities.get(i).getSpace());
        ((TextView) myView.findViewById(R.id.txtFacilityName)).setText(facilities.get(i).getName());
        ((TextView) myView.findViewById(R.id.txtDescription)).setText(facilities.get(i).getDescription());
        ((TextView) myView.findViewById(R.id.txtInchargeEmail)).setText(
                facilities.get(i).getSecurityEmail() != null ?  facilities.get(i).getSecurityEmail(): "No in-charge assigned");

        if (!"User".equals(userRole)) {

            if ("Manager".equals(userRole) && facilities.get(i).getSecurityEmail() == null)
                myView.findViewById(R.id.btnAssignIncharge).setVisibility(View.VISIBLE);

            myView.findViewById(R.id.btnFacilityBook).setVisibility(View.GONE);
            myView.findViewById(R.id.ltAdminManager).setVisibility(View.VISIBLE);
            myView.findViewById(R.id.btnEditFacility).setOnClickListener(view1 -> {

                View diag_view = LayoutInflater.from(context).inflate(R.layout.activity_edit_facility, null);
                AlertDialog builder = new AlertDialog.Builder(context).create();

                EditText txtFacilityName = diag_view.findViewById(R.id.txtFacilityName),
                        txtFacilityDescription = diag_view.findViewById(R.id.txtFacilityDescription),
                        txtFacilifyCapacity = diag_view.findViewById(R.id.txtFacilityCapacity),
                        txtFacilityPrice = diag_view.findViewById(R.id.txtFacilityPrice);

                builder.setView(diag_view);
                builder.setCanceledOnTouchOutside(true);
                txtFacilityName.setText(facilities.get(i).getName());
                txtFacilityDescription.setText(facilities.get(i).getDescription());
                txtFacilifyCapacity.setText(facilities.get(i).getSpace());
                txtFacilityPrice.setText(String.valueOf(facilities.get(i).getPrice()));
                diag_view.findViewById(R.id.btnUpdateFacility).setOnClickListener(v3 -> {

                    String sqlQuery = "update Facility " +
                            "set Name = '" + txtFacilityName.getText() + "'," +
                            " Description = '" + txtFacilityDescription.getText() + "', " +
                            "Price = '" + txtFacilityPrice.getText() + "', " +
                            "Space = '" + txtFacilifyCapacity.getText() + "' " +
                            "where FacilityId = " + facilities.get(i).getFacilityId() + "";

                    try {
                        Statement statement = connection.createStatement();

                        int numColsAffected = statement.executeUpdate(sqlQuery);
                        if (numColsAffected > 0) {
                            Toast.makeText(context, "Facility updated successfully.", Toast.LENGTH_LONG).show();
                            builder.dismiss();
                        } else {
                            Toast.makeText(context,
                                    "Couldn't update facility.", Toast.LENGTH_LONG).show();
                            builder.dismiss();
                        }
                    } catch (Exception e) {
                        Toast.makeText(context,
                                "Database error. Please contact system administrator.", Toast.LENGTH_LONG).show();
                        builder.dismiss();
                        e.printStackTrace();
                    }
                });
                diag_view.findViewById(R.id.btnGoBackToFacilities).setOnClickListener(v3 -> {
                    builder.dismiss();
                });
                builder.show();
            });
            myView.findViewById(R.id.btnDeleteFacility).setOnClickListener(view1 -> {
                new AlertDialog.Builder(context)
                        .setMessage("Are you sure you want to delete this facility?")
                        .setTitle("Delete facility?")
                        .setNegativeButton("Yes", (dialogInterface, i1) -> {

                            String sqlQuery = "delete from Facility " +
                                    "where FacilityId = " + facilities.get(i).getFacilityId() + "";

                            try {
                                Statement statement = connection.createStatement();

                                int numColsAffected = statement.executeUpdate(sqlQuery);
                                if (numColsAffected > 0) {
                                    Toast.makeText(context, "Facility deleted successfully.", Toast.LENGTH_LONG).show();
                                } else {
                                    Toast.makeText(context,
                                            "Couldn't delete facility.", Toast.LENGTH_LONG).show();
                                }
                            } catch (Exception e) {
                                Toast.makeText(context,
                                        "Database error. Please contact system administrator.", Toast.LENGTH_LONG).show();
                                e.printStackTrace();
                            }
                        })
                        .setPositiveButton("Cancel", (dialogInterface, i12) -> dialogInterface.cancel())
                        .show();
            });

            myView.findViewById(R.id.btnAssignIncharge).setOnClickListener(view1 -> {
                ArrayList<User> users = new ArrayList<>();
                try {
                    Statement statement = connection.createStatement();
                    ResultSet user_set = statement.executeQuery(
                            "select * from AspNetUsers " +
                                    "join AspNetUserRoles on AspNetUserRoles.UserId = AspNetUsers.Id " +
                                    "join AspNetRoles on AspNetRoles.Id = AspNetUserRoles.RoleId " +
                                    "where AspNetRoles.Name like '%user%'"
                    );
// TODO: 2023/10/23 delete user from user role add to secuti role 
                    while (user_set.next()) {
                        User user = new User(null, null, null, null, null, null, null);
                        user.setId(user_set.getString(1));
                        user.setFirstName(user_set.getString(2));
                        user.setLastName(user_set.getString(3));
                        user.setNormalizedEmail(user_set.getString(12));
                        user.setIdNumber(user_set.getString(4));
                        user.setStudentNumber(user_set.getString(7));
                        user.setUsername(user_set.getString(10));
                        users.add(user);
                    }

                     new AlertDialog.Builder(context)
                            .setTitle("Assign facility in-charge")
                            .setAdapter(new ArrayAdapter<User>(context,
                                    android.R.layout.simple_list_item_1, users), (dialogInterface, i13) -> {

                                String sqlQuery = "update Facility " +
                                        "set SecurityEmail = '" + users.get(i13).getNormalizedEmail().toLowerCase() + "' " +
                                        "where FacilityId = " + facilities.get(i).getFacilityId() + "";
                                try {
                                    int numColsAffected = statement.executeUpdate(sqlQuery);
                                    if (numColsAffected > 0) {
                                        Toast.makeText(context, "Facility updated successfully.", Toast.LENGTH_LONG).show();
                                    } else {
                                        Toast.makeText(context,
                                                "Couldn't update facility.", Toast.LENGTH_LONG).show();
                                    }
                                } catch (Exception e) {
                                    Toast.makeText(context,
                                            "Database error. Please contact system administrator.", Toast.LENGTH_LONG).show();
                                    e.printStackTrace();
                                }
                            }).show();

                } catch (Exception e) {
                    e.printStackTrace();
                }


            });
        } else {
            myView.findViewById(R.id.btnFacilityBook).setVisibility(View.VISIBLE);
            myView.findViewById(R.id.ltAdminManager).setVisibility(View.GONE);

            myView.findViewById(R.id.btnFacilityBook).setOnClickListener(v -> {
                Intent booking = new Intent(context, MakeBookingActivity.class);
                booking.putExtra("facility", (Serializable) facilities.get(i));
                booking.putExtra("userEmail", userData.getUserEmail());

                booking.putExtra("categories", (Serializable) categories);
                startActivity(context, booking, null);
            });
        }

        return myView;
    }
}