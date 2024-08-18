package mncedisi.mlangeni.myrecapp.adapters;

import static androidx.core.content.ContextCompat.startActivity;

import android.content.Context;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.BaseAdapter;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.Statement;
import java.util.ArrayList;

import mncedisi.mlangeni.myrecapp.R;
import mncedisi.mlangeni.myrecapp.data.Category;
import mncedisi.mlangeni.myrecapp.data.Facility;
import mncedisi.mlangeni.myrecapp.data.Notification;
import mncedisi.mlangeni.myrecapp.data.User;
import mncedisi.mlangeni.myrecapp.ui.book.MakeBookingActivity;

public class NotificationAdapter extends BaseAdapter {

    private final Context context;
    private ArrayList<Notification> notifications;


    public NotificationAdapter(Context context, ArrayList<Notification> notifications) {
        this.context = context;
        this.notifications = notifications;
    }

    @Override
    public int getCount() {
        return notifications.size();
    }


    @Override
    public Object getItem(int i) {
        return notifications.get(i);
    }

    @Override
    public long getItemId(int i) {
        return i;
    }

    @Override
    public View getView(int i, View view, ViewGroup viewGroup) {
        View myView = View.inflate(context, R.layout.app_notification_item, null);
        ((TextView) myView.findViewById(R.id.txtNotificationText)).setText(notifications.get(i).getMessage());
        return myView;
    }
}