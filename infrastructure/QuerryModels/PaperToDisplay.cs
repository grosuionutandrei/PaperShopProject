﻿using System.Collections;

namespace infrastructure.QuerryModels;

public class PaperToDisplay
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool Discontinued { get; set; }

    public int Stock { get; set; }

    public double Price { get; set; }

    public IEnumerable? PaperPropertiesList { get; set; }


    public void IncludeProperties(IEnumerable<PaperProperties> propertiesList)
    {
        PaperPropertiesList = propertiesList;
    }
}