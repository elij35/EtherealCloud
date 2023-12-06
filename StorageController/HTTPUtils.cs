using Azure.Core;

namespace StorageController
{
    public class HTTPUtils
    {

        public static Dictionary<string, string> ConvertParamsToDictionary(Stream request, int content_length)
        {

            // Stream buffer to read all the content bytes into
            byte[] stream_buffer = new byte[content_length];

            // Asynchrounously reading all the char bytes from the request body
            request.ReadAsync(stream_buffer, 0, content_length);

            // Convering the bytes into characters and adding them to a string
            string body_string = string.Empty;
            foreach (byte character in stream_buffer)
                body_string += (char)character;

            /*
             * 
             * Form parameters in format: key=value&key1=value1 and so on.
             * this converts them into a dictionary of keys and values to be used
             * 
             * splits at the & to get each key-value pair
             * 
             * loops of key-value pairs, splits them and adds them to the dictonary
             * 
             */

            // Dictionary for key-value pairs
            Dictionary<string, string> form_params = new Dictionary<string, string>();

            // Key value pair array
            string[] param_pairs = body_string.Split('&');

            // Looping over each key-value pair
            foreach (string param in param_pairs)
            {
                // Splitting each key value at the '=' as they're in the format key=value
                string[] pair = param.Split('=');
                form_params.Add(pair[0], pair[1]);
            }

            // Returning the dictionary of key-value pairs
            return form_params;

        }

    }
}
