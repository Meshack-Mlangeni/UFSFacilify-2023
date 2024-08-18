package mncedisi.mlangeni.myrecapp.ui.book;

import androidx.appcompat.app.AppCompatActivity;
import mncedisi.mlangeni.myrecapp.MainActivity;
import mncedisi.mlangeni.myrecapp.R;
import mncedisi.mlangeni.myrecapp.ui.home.HomeFragment;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;

public class CancelActivity extends AppCompatActivity {
    Button btnFailGoHome;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_cancel);

        btnFailGoHome = findViewById(R.id.btnFailReturnToHomePage);
        btnFailGoHome.setOnClickListener(view -> {
            finish();
        });
    }
}