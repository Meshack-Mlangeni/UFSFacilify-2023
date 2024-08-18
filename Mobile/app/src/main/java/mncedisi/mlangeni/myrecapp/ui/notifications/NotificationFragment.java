package mncedisi.mlangeni.myrecapp.ui.notifications;

import android.app.ProgressDialog;
import android.content.Context;
import android.media.Image;
import android.os.Bundle;
import android.os.Handler;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.RecyclerView;

import com.google.android.material.snackbar.Snackbar;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;

import javax.xml.transform.Result;

import mncedisi.mlangeni.myrecapp.R;
import mncedisi.mlangeni.myrecapp.adapters.CreateConnection;
import mncedisi.mlangeni.myrecapp.adapters.NotificationAdapter;
import mncedisi.mlangeni.myrecapp.adapters.UserData;
import mncedisi.mlangeni.myrecapp.data.Notification;
import mncedisi.mlangeni.myrecapp.data.User;

public class NotificationFragment extends Fragment {
    ResultSet notification;
    ListView lstNotifications;
    CreateConnection createConnection;
    ArrayList<Notification> userNotifications;
    Connection connection;
    UserData data;
    ProgressDialog dialog;
    Handler handler = new Handler();

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {
        createConnection = new CreateConnection(getActivity());
        dialog = ProgressDialog.show(getActivity(),"Loading notifications", "Please wait...");
        return inflater.inflate(R.layout.fragment_notifications, container, false);
    }

    ImageView imgNoNotification;

    @Override
    public void onStart() {
        super.onStart();
        new Thread(() -> {
            this.connection = createConnection.getConnection();
            handler.post(() -> {

                data = (UserData) requireActivity().getIntent().getSerializableExtra("data");
                notification = createConnection.getData("SELECT * FROM Notification where UserEmail = '" + data.getUserEmail() + "';");
                userNotifications = Notification.createUserNotificationsFromSet(notification);

                lstNotifications = getActivity().findViewById(R.id.lstNotifications);
                lstNotifications.setDivider(null);
                lstNotifications.setAdapter(new NotificationAdapter(getActivity(),
                        userNotifications));

                imgNoNotification = getActivity().findViewById(R.id.imgNoNotification);

                if (lstNotifications.getCount() == 0) {
                    imgNoNotification.setVisibility(View.VISIBLE);
                }
                dialog.dismiss();
            });
        }).start();
    }
}