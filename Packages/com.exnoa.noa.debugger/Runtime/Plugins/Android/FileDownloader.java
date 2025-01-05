package NoaDebugger;

import android.app.Activity;
import android.app.Fragment;
import android.app.FragmentTransaction;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;

import com.unity3d.player.UnityPlayer;

import java.io.OutputStream;

public class FileDownloader extends Fragment
{
    private static final String TAG = "unimgpicker";
    private static final int CREATE_FILE = 1002;

    public String DataText = "";
    public String FileName = "";
    public String Type = "";
    public static int Status = 0;

    public static void downloadFile(String fileName, String dataText, String type)
    {
        Activity unityActivity = UnityPlayer.currentActivity;

        FileDownloader downloader = new FileDownloader();
        downloader.DataText = dataText;
        downloader.FileName = fileName;
        downloader.Type = type;
        downloader.Status = 0;

        FragmentTransaction transaction = unityActivity.getFragmentManager().beginTransaction();

        transaction.add(downloader, TAG);
        transaction.commit();
    }

    @Override
    public void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        _DownloadFile();
    }

    private void _DownloadFile()
    {
        Intent intent = new Intent(Intent.ACTION_CREATE_DOCUMENT);
        intent.addCategory(Intent.CATEGORY_OPENABLE);
        intent.setType(Type);
        intent.putExtra(Intent.EXTRA_TITLE, FileName);

        startActivityForResult(intent, CREATE_FILE);
    }

    public static int downloadStatus()
    {
        return Status;
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent resultData)
    {
        super.onActivityResult(requestCode, resultCode,resultData );

        if(resultCode == Activity.RESULT_OK)
        {
            if(resultData.getData() == null)
            {
                Status = -1;
                return;
            }

            Uri uri = resultData.getData();
            Context context = getActivity().getApplicationContext();

            if (requestCode == CREATE_FILE)
            {
                try(OutputStream outputStream = context.getContentResolver().openOutputStream(uri))
                {
                    if(outputStream != null)
                    {
                        outputStream.write(DataText.getBytes());
                        Status = 1;
                        return;
                    }
                }
                catch(Exception e)
                {
                    Status = -1;
                }
                return;
            }
        }
        Status = -1;
    }
}
