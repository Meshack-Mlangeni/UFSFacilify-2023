package mncedisi.mlangeni.myrecapp.adapters;

import android.content.Context;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.google.android.material.chip.Chip;

import java.util.ArrayList;
import java.util.Date;

import mncedisi.mlangeni.myrecapp.R;
import mncedisi.mlangeni.myrecapp.data.Booking;

public class UserBookingAdapter extends BaseAdapter
{
    public UserBookingAdapter(Context context, ArrayList<Booking> _bookings) {
        this.context = context;
        bookings = _bookings;
    }

    private final Context context;
    private ArrayList<Booking> bookings;

    @Override
    public int getCount() {
        return bookings.size();
    }

    @Override
    public Object getItem(int i) {
        return bookings.get(i);
    }

    @Override
    public long getItemId(int i) {
        return i;
    }

    @Override
    public View getView(int i, View view, ViewGroup viewGroup) {
        View myView = View.inflate(context, R.layout.app_menu_item, null);


        ((TextView)myView.findViewById(R.id.txtBookingFacilityName)).setText(bookings.get(i).getFacilityName());
        ((TextView)myView.findViewById(R.id.txtBookingCategoryName)).setText(bookings.get(i).getCategoryName());
         ((TextView)myView.findViewById(R.id.txtDate))
                .setText(bookings.get(i).getDateStart().toString().substring(0,10));
        ((Chip)myView.findViewById(R.id.chipBookingPass)).setText(bookings.get(i).getBookingPass());



        ((Chip)myView.findViewById(R.id.chipFromTo))
                .setText(bookings.get(i).getDateStart().getHours() + ":" + bookings.get(i).getDateStart().getMinutes()
                 + " - " + bookings.get(i).getDateEnd().getHours() + ":" + bookings.get(i).getDateEnd().getMinutes() );

        return myView;
    }
}
