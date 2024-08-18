package mncedisi.mlangeni.myrecapp.ui.home;

import android.app.ProgressDialog;
import android.os.Bundle;
import android.os.Handler;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;


import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;

import mncedisi.mlangeni.myrecapp.R;
import mncedisi.mlangeni.myrecapp.adapters.CreateConnection;
import mncedisi.mlangeni.myrecapp.adapters.UserBookingAdapter;
import mncedisi.mlangeni.myrecapp.adapters.UserData;
import mncedisi.mlangeni.myrecapp.data.Booking;
import mncedisi.mlangeni.myrecapp.data.Notification;
import mncedisi.mlangeni.myrecapp.data.User;

public class HomeFragment extends Fragment {

    UserBookingAdapter adpMenu;
    ListView lstItem;
    private User logged_user;
    private UserData data;
    private ArrayList<Booking> userBookings;
    private CreateConnection createConnection;
    ResultSet roleset, user, bookings;
    String Role;
    Handler handler = new Handler();
    Connection connection;
    ProgressDialog dialog;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {
        createConnection = new CreateConnection(getActivity());
        dialog = ProgressDialog.show(getActivity(), "", "Please wait...");
        return inflater.inflate(R.layout.fragment_home, container, false);
    }

    String sGreet;
    ArrayAdapter<String> arrayAdapter;

    @Override
    public void onStart() {
        super.onStart();

        new Thread(() -> {
            this.connection = createConnection.getConnection();
            handler.post(new Runnable() {
                @Override
                public void run() {
                    data = (UserData) requireActivity().getIntent().getSerializableExtra("data");
                    user = createConnection.getData("SELECT * FROM AspNetUsers where NormalizedEmail = '" + data.getUserEmail().toUpperCase() + "';");
                    logged_user = User.createUserFromResultSet(user);
                    bookings = createConnection.getData("SELECT * FROM Booking where userEmail = '" + data.getUserEmail() + "';");
                    userBookings = Booking.createUserBookingsFromSet(bookings, connection);

                    roleset = createConnection.getData("select Name from AspNetUserRoles join AspNetUsers " +
                            "on AspNetUsers.Id = UserId " +
                            "join AspNetRoles on " +
                            "AspNetRoles.Id = AspNetUserRoles.RoleId " +
                            "where AspNetUsers.Id = '" + logged_user.getId() + "'");
                    Role = "User";
                    try {
                        while (roleset.next()) Role = roleset.getString(1);
                    } catch (SQLException e) {
                        e.printStackTrace();
                    }

                    lstItem = getActivity().findViewById(R.id.lstUserBookings);
                    lstItem.setDivider(null);
                    sGreet = "Hello " + logged_user.getFirstName() + " " + logged_user.getLastName();
                    ((TextView) getActivity().findViewById(R.id.txtGreetUser)).setText(sGreet);

                    if ("Administrator".equals(Role)) {
                        arrayAdapter = new ArrayAdapter<>
                                (getActivity(), android.R.layout.simple_list_item_1,
                                        getResources().getStringArray(R.array.admin));
                        ((TextView) getActivity().findViewById(R.id.txtTodayPick))
                                .setText("CONTROL PANEL");
                        getActivity().findViewById(R.id.txtNoBookings).setVisibility(View.GONE);
                        getActivity().findViewById(R.id.imgNoBooking).setVisibility(View.GONE);
                        lstItem.setAdapter(arrayAdapter);
                        lstItem.setOnItemClickListener(this::onItemClick);

                    } else if ("Manager".equals(Role)) {
                        arrayAdapter = new ArrayAdapter<>
                                (getActivity(), android.R.layout.simple_list_item_1,
                                        getResources().getStringArray(R.array.manager));
                        ((TextView) getActivity().findViewById(R.id.txtTodayPick))
                                .setText("CONTROL PANEL");
                        getActivity().findViewById(R.id.txtNoBookings).setVisibility(View.GONE);
                        getActivity().findViewById(R.id.imgNoBooking).setVisibility(View.GONE);
                        lstItem.setAdapter(arrayAdapter);
                        lstItem.setOnItemClickListener(this::onItemClick);
                    } else {
                        if ("Manager".equals(Role)) {
                            userBookings = Booking.createUserBookingsFromSet
                                    (createConnection.getData("select * from booking"), createConnection.getConnection());
                        }
                        adpMenu = new UserBookingAdapter(getActivity(), userBookings);
                        lstItem.setAdapter(adpMenu);
                        if (adpMenu.getCount() == 0) {
                            getActivity().findViewById(R.id.imgNoBooking).setVisibility(View.VISIBLE);
                            getActivity().findViewById(R.id.txtNoBookings).setVisibility(View.VISIBLE);
                            lstItem.setVisibility(View.GONE);
                        } else {
                            getActivity().findViewById(R.id.imgNoBooking).setVisibility(View.GONE);
                            getActivity().findViewById(R.id.txtNoBookings).setVisibility(View.GONE);
                            lstItem.setVisibility(View.VISIBLE);
                        }
                    }
                    dialog.dismiss();
                }

                private void onItemClick(AdapterView<?> adapterView, View view, int i, long l)
                {

                }
            });
        }).start();
    }
}