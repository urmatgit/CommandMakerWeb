using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
public partial class GameDto
{
    public TimeSpan? TimeGame
    {
        get
        {
            if (!string.IsNullOrEmpty(Time))
                return TimeSpan.Parse(Time);
            else return default(TimeSpan);
        }
        set
        {
            Time = value!.Value.ToString();
        }
    }
}

public partial class UpdateGameRequest
    {
    public TimeSpan? TimeGame
    {
        get
        {
            if (!string.IsNullOrEmpty(Time))
                return TimeSpan.Parse(Time);
            else return default(TimeSpan);
        }
        set
        {
            Time = value!.Value.ToString();
        }
    }

}