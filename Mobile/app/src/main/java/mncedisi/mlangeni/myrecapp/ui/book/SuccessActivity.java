package mncedisi.mlangeni.myrecapp.ui.book;

import androidx.appcompat.app.AppCompatActivity;
import mncedisi.mlangeni.myrecapp.MainActivity;
import mncedisi.mlangeni.myrecapp.R;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.TextView;

// TODO: 2023/10/20 txtsuccess 
public class SuccessActivity extends AppCompatActivity {

    Button btnSuccessGoHome;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_success);

        ((TextView) findViewById(R.id.txtSuccessPass)).setText(getIntent().getStringExtra("pass"));
        btnSuccessGoHome = findViewById(R.id.btnSuccessReturnToHomePage);
        btnSuccessGoHome.setOnClickListener(view -> {
            finish();
        });
    }
}