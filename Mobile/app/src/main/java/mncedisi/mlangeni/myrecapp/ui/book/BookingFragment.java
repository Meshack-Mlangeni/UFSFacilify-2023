package mncedisi.mlangeni.myrecapp.ui.book;

import android.app.ProgressDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.ListView;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AlertDialog;
import androidx.fragment.app.Fragment;

import com.google.android.material.floatingactionbutton.ExtendedFloatingActionButton;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.text.SimpleDateFormat;
import java.util.ArrayList;

import mncedisi.mlangeni.myrecapp.R;
import mncedisi.mlangeni.myrecapp.adapters.CreateConnection;
import mncedisi.mlangeni.myrecapp.adapters.FacilityAdapter;
import mncedisi.mlangeni.myrecapp.adapters.UserData;
import mncedisi.mlangeni.myrecapp.data.Category;
import mncedisi.mlangeni.myrecapp.data.Facility;
import mncedisi.mlangeni.myrecapp.data.User;

public class BookingFragment extends Fragment {

    ListView lstFacilities;
    ImageView imgNothing;

    private Connection connection;
    CreateConnection createConnection;
    private ResultSet facilities, roleset;
    private UserData data;
    private ArrayList<Facility> app_facilities;
    private ArrayList<Category> categories;
    ProgressDialog dialog;
    ExtendedFloatingActionButton flCreateNew;
    String Role;
    Handler handler = new Handler();

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        createConnection = new CreateConnection(getActivity());
        dialog = ProgressDialog.show(getActivity(), "Loading Facilities", "Please wait...");
        return inflater.inflate(R.layout.fragment_booking, container, false);
    }

    @Override
    public void onStart() {
        super.onStart();

        new Thread(() -> {
            this.connection = createConnection.getConnection();
            handler.post(new Runnable() {
                @Override
                public void run() {
                    data = (UserData) requireActivity().getIntent().getSerializableExtra("data");
                    facilities = createConnection.getData("SELECT * FROM Facility");
                    categories = Category.createCategoriesFromResultSet(createConnection.getData("SELECT * FROM category"));
                    app_facilities = Facility.createFacilitiesFromResultSet(facilities, connection);

                    lstFacilities = getActivity().findViewById(R.id.lstFacilities);
                    lstFacilities.setDivider(null);
                    imgNothing = getActivity().findViewById(R.id.imgNothing);
                    flCreateNew = getActivity().findViewById(R.id.btnAdminCreatefaciliy);

                    roleset = createConnection.getData("select Name from AspNetUserRoles join AspNetUsers " +
                            "on AspNetUsers.Id = UserId " +
                            "join AspNetRoles on " +
                            "AspNetRoles.Id = AspNetUserRoles.RoleId " +
                            "where AspNetUsers.Id = '" + data.getUserId() + "'");
                    Role = "User";
                    try {
                        while (roleset.next())   Role = roleset.getString(1);
                    } catch (SQLException e) {
                        e.printStackTrace();
                    }

                    FacilityAdapter adapter =
                            new FacilityAdapter(getActivity(), app_facilities,
                                    categories, data, Role, connection);
                    lstFacilities.setAdapter(adapter);

                    if (lstFacilities.getCount() == 0)
                        imgNothing.setVisibility(View.VISIBLE);

                    dialog.dismiss();
                }
            });
        }).start();
    }
}